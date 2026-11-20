using learingASP.Data;
using learingASP.Models;
using Microsoft.EntityFrameworkCore;

namespace learingASP.Repositories.Employees
{
    public class EmployeesRepository(AppDbContext context) : IEmployeesRepository
    {
        public async Task<List<Employee>> GetActiveByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();

            return await context.Employees
                .AsNoTracking()
                .Where(e => idList.Contains(e.Id) && e.IsActive)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(bool includeInactive)
           => await context.Employees
            .AsNoTracking()                                
            .Where(e => includeInactive || e.IsActive)
            .ToListAsync();

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
           => await context.Employees.FindAsync(id);

        public void AddEmployee(Employee employee)
          => context.Employees.Add(employee);

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Employees
                .AnyAsync(e => e.Email == email);
        }
        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
