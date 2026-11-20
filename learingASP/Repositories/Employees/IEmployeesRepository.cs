using learingASP.Models;
using Microsoft.EntityFrameworkCore;

namespace learingASP.Repositories.Employees
{
    public interface IEmployeesRepository
    {
        Task<List<Employee>> GetAllEmployeesAsync(bool includeInactive);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        void AddEmployee(Employee employee);
        Task<bool> EmailExistsAsync(string email);
        Task<Employee?> GetEmployeeByEmailAsync(string email);
    }
}
