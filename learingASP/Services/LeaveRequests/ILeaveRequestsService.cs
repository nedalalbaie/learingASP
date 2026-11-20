using learingASP.Common;
using learingASP.Dtos.LeaveRequests;
using learingASP.Models;

namespace learingASP.Services.LeaveRequests
{
    public interface ILeaveRequestsService
    {
        Task<Result<List<LeaveRequestResponseDto>>> GetAllLeaveRequestsAsync();

        Task<Result<LeaveRequestResponseDto>>
            CreateLeaveRequestAsync(int employeeId, CreateLeaveRequestDto LeaveRequest);

        Task<Result<LeaveRequestResponseDto>>
           AdminCreateLeaveRequestAsync(int adminId, AdminCreateLeaveRequestDto LeaveRequest);


        Task<Result>
            ApproveLeaveRequestAsync(int leaveRequestId, int adminId);

        Task<Result>
            RejectLeaveRequestAsync(int leaveRequestId, int adminId);

        Task<Result<LeaveBalanceDto>> GetEmployeeLeaveBalanceAsync(int employeeId, int? year);

        Task<Result<List<LeaveRequestResponseDto>>>
            GetEmployeeLeaveRequestsAsync(int employeeId);
     }
}
