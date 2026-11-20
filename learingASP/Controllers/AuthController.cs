using learingASP.Common;
using learingASP.Dtos.Auth;
using learingASP.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learingASP.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponseDto>> Refresh(
            RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
        {
            var result = await authService.LogoutAsync(request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return NoContent();
        }
    }
}
