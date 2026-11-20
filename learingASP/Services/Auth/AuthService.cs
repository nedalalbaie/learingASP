using learingASP.Common;
using learingASP.Configurations;
using learingASP.Dtos.Auth;
using learingASP.Models;
using learingASP.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace learingASP.Services.Auth
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<Employee> passwordHasher,
        IOptions<JwtSettings> jwtOptions) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var employee = await unitOfWork.EmployeesRepository
                .GetEmployeeByEmailAsync(email);

            if (employee is null || !employee.IsActive)
            {
                return Error.Validation("Invalid email or password.");
            }

            var passwordResult = passwordHasher.VerifyHashedPassword(
                employee,
                employee.PasswordHash,
                request.Password
            );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Error.Validation("Invalid email or password.");
            }

            var accessToken = GenerateAccessToken(employee);
            var refreshTokenValue = GenerateRefreshToken();

            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);

            var refreshToken = new RefreshToken
            {
                EmployeeId = employee.Id,
                TokenHash = HashRefreshToken(refreshTokenValue),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            };

            unitOfWork.RefreshTokensRepository.AddRefreshToken(refreshToken);
            await unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                    _jwtSettings.AccessTokenExpirationMinutes),
                EmployeeId = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc
            };
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var tokenHash = HashRefreshToken(request.RefreshToken);

            var storedToken = await unitOfWork.RefreshTokensRepository
                .GetByTokenHashAsync(tokenHash);

            if (storedToken is null)
            {
                return Error.Validation("Invalid refresh token.");
            }

            var now = DateTime.UtcNow;

            if (storedToken.RevokedAtUtc.HasValue ||
                storedToken.ExpiresAtUtc <= now ||
                !storedToken.Employee.IsActive)
            {
                return Error.Validation("Invalid refresh token.");
            }

            var employee = storedToken.Employee;

            storedToken.RevokedAtUtc = now;

            var newRefreshTokenValue = GenerateRefreshToken();
            var newRefreshTokenExpiresAtUtc = now.AddDays(
             _jwtSettings.RefreshTokenExpirationDays);

            var newRefreshToken = new RefreshToken
            {
                EmployeeId = employee.Id,
                TokenHash = HashRefreshToken(newRefreshTokenValue),
                CreatedAtUtc = now,
                ExpiresAtUtc = newRefreshTokenExpiresAtUtc
            };

            unitOfWork.RefreshTokensRepository.AddRefreshToken(newRefreshToken);

            var accessToken = GenerateAccessToken(employee);
            var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

            await unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresAtUtc = accessTokenExpiresAtUtc,

                RefreshToken = newRefreshTokenValue,
                RefreshTokenExpiresAtUtc = newRefreshTokenExpiresAtUtc,

                EmployeeId = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role.ToString()
            };

        }

        private string GenerateAccessToken(Employee employee)
        {
            var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                    new(ClaimTypes.Name, employee.Name),
                    new(ClaimTypes.Email, employee.Email),
                    new(ClaimTypes.Role, employee.Role.ToString())
                };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result> LogoutAsync(RefreshTokenRequestDto request)
        {
            var tokenHash = HashRefreshToken(request.RefreshToken);

            var storedToken = await unitOfWork.RefreshTokensRepository
                .GetByTokenHashAsync(tokenHash);

            if (storedToken is null || storedToken.RevokedAtUtc.HasValue)
                return Error.Validation("Invalid refresh token.");

            storedToken.RevokedAtUtc = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

            return Convert.ToHexString(bytes);
        }
    }
}
