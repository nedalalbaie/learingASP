using learingASP.Data;
using learingASP.Repositories.Employees;
using learingASP.Repositories.LeaveRequests;
using learingASP.Repositories.RefreshTokens;

namespace learingASP.Repositories
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public IEmployeesRepository EmployeesRepository { get; } = new EmployeesRepository(context);
        public ILeaveRequestsRepository LeaveRequestsRepository { get; } = new LeaveRequestsRepository(context);

        public IRefreshTokensRepository RefreshTokensRepository { get; } = new RefreshTokensRepository(context);

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
    }
}
