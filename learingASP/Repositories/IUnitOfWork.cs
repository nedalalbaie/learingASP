using learingASP.Repositories.Employees;
using learingASP.Repositories.LeaveRequests;
using learingASP.Repositories.RefreshTokens;

namespace learingASP.Repositories
{
    public interface IUnitOfWork
    {
        IEmployeesRepository EmployeesRepository { get; }
        ILeaveRequestsRepository LeaveRequestsRepository { get; }
        IRefreshTokensRepository RefreshTokensRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
