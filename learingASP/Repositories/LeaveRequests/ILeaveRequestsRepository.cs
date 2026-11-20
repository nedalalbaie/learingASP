using learingASP.Models;

namespace learingASP.Repositories.LeaveRequests
{
    public interface ILeaveRequestsRepository
    {
   
        Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        void AddLeaveRequestAsync(LeaveRequest leaveRequest);

        Task<decimal> GetApprovedLeaveRequestDaysAsync(int employeeId, int year);

        Task<decimal> GetPendingLeaveRequestDaysAsync(int employeeId, int year);

        Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId);

        Task<bool> HasOverlappingLeaveRequestAsync(
            int employeeId,
            DateOnly startDate,
            DateOnly endDate);
    }
}
