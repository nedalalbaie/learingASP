using learingASP.Data;
using learingASP.Enums;
using learingASP.Models;
using Microsoft.EntityFrameworkCore;

namespace learingASP.Repositories.LeaveRequests
{
    public class LeaveRequestsRepository(AppDbContext context) : ILeaveRequestsRepository
    {
        public async Task<List<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await context.LeaveRequests
                .AsNoTracking()
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int leaveRequestId)
        {
            return await context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == leaveRequestId);
        }

        public void AddLeaveRequestAsync(LeaveRequest leaveRequest)
         => context.LeaveRequests.Add(leaveRequest);

        public async Task<decimal> GetApprovedLeaveRequestDaysAsync(
           int employeeId,
           int year)
        {
            var yearStart = new DateOnly(year, 1, 1);
            var yearEnd = new DateOnly(year, 12, 31);

            return await context.LeaveRequests
                .Where(l =>
                    l.EmployeeId == employeeId &&
                    l.Status == LeaveStatus.Approved &&
                    l.StartDate >= yearStart &&
                    l.StartDate <= yearEnd)
                .SumAsync(l => l.Days);
        }

        public async Task<decimal> GetPendingLeaveRequestDaysAsync(
            int employeeId,
            int year)
        {
            var yearStart = new DateOnly(year, 1, 1);
            var yearEnd = new DateOnly(year, 12, 31);

            return await context.LeaveRequests
                .Where(l =>
                    l.EmployeeId == employeeId &&
                    l.Status == LeaveStatus.Pending &&
                    l.StartDate >= yearStart &&
                    l.StartDate <= yearEnd)
                .SumAsync(l => l.Days);
        }

        public async Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId)
        {
            return await context.LeaveRequests
                .AsNoTracking()
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingLeaveRequestAsync(
            int employeeId,
            DateOnly startDate,
            DateOnly endDate)
        {
            return await context.LeaveRequests
                .AnyAsync(l =>
                    l.EmployeeId == employeeId &&
                    l.Status != LeaveStatus.Rejected &&
                    l.StartDate <= endDate &&
                    l.EndDate >= startDate);
        }
    }
}
