using learingASP.Common;
using learingASP.Dtos.Auth;

namespace learingASP.Services.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(
             RefreshTokenRequestDto request);

        Task<Result> LogoutAsync(RefreshTokenRequestDto request);
    }
}
