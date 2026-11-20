using learingASP.Common;
using learingASP.Dtos.LeaveRequests;
using learingASP.Services.LeaveRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace learingASP.Controllers
{
    [Route("leave-requests")]
    [ApiController]
    [Authorize]
    public class LeaveRequestsController(ILeaveRequestsService leaveRequestsService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LeaveRequestResponseDto>>> GetAllLeaveRequests()
        {
            var result = await leaveRequestsService.GetAllLeaveRequestsAsync();

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpPost()]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<LeaveRequestResponseDto>> CreateLeaveRequest(CreateLeaveRequestDto request)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
               out var employeeId))
            {
                return Unauthorized();
            }

            var result = await leaveRequestsService.CreateLeaveRequestAsync(employeeId, request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return CreatedAtAction(
                 nameof(GetAllLeaveRequests),
                 new { id = result.Value.Id },
                 result.Value
             );
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LeaveRequestResponseDto>> AdminCreateLeaveRequest(AdminCreateLeaveRequestDto request)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
               out var adminId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");

            var result = await leaveRequestsService.AdminCreateLeaveRequestAsync(adminId, request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return CreatedAtAction(
               nameof(GetAllLeaveRequests),
               new { id = result.Value.Id },
               result.Value
           );
        }

        [HttpPost("{leaveRequestId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LeaveRequestResponseDto>> ApproveLeaveRequest(int leaveRequestId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var adminId))
            {
                return Unauthorized();
            }

            var result = await leaveRequestsService.ApproveLeaveRequestAsync(leaveRequestId, adminId);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }
               
            return NoContent();
        }

        [HttpPost("{leaveRequestId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LeaveRequestResponseDto>> RejectLeaveRequest(int leaveRequestId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var adminId))
            {
                return Unauthorized();
            }

            var result = await leaveRequestsService.RejectLeaveRequestAsync(leaveRequestId, adminId);

            if (!result.IsSuccess)
                return this.ToProblem(result);

            return NoContent();
        }

        [HttpGet("~/employees/{employeeId}/leave-requests")]
        public async Task<ActionResult<List<LeaveRequestResponseDto>>> GetEmployeeLeaveRequests(int employeeId)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var currentUserId))
            {
                return Unauthorized();
            }

            if (User.IsInRole("Employee") && currentUserId != employeeId)
            {
                return Forbid();
            }

            var result = await leaveRequestsService
                .GetEmployeeLeaveRequestsAsync(employeeId);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }

        [HttpGet("~/employees/{employeeId}/leave-balance")]
        public async Task<ActionResult<LeaveBalanceDto>> GetEmployeeLeaveBalance(int employeeId, [FromQuery] int? year)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var currentUserId))
            {
                return Unauthorized();
            }

            if (User.IsInRole("Employee") && currentUserId != employeeId)
            {
                return Forbid();
            }

            var result = await leaveRequestsService
                .GetEmployeeLeaveBalanceAsync(employeeId, year);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }
    }
}
