using learingASP.Common;
using learingASP.Dtos;
using learingASP.Dtos.Employees;

namespace learingASP.Services.Employees
{
    public interface IEmployeesService
    {
        Task<List<EmployeeResponseDto>> GetAllEmployeesAsync(bool includeInactive);
        Task<Result<EmployeeResponseDto?>> GetEmployeeByIdAsync(int id);
        Task<Result<EmployeeResponseDto>> CreateEmployeeAsync(CreateEmployeeDto employeeDto);

        Task<Result> UpdateEmployeeAsync(int id, CreateEmployeeDto employeeDto);

        Task<Result> DeactivateEmployeeAsync(int id);
    }
}
