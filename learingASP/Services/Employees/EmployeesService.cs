using learingASP.Common;
using learingASP.Dtos.Employees;
using learingASP.Models;
using learingASP.Repositories;
using Microsoft.AspNetCore.Identity;

namespace learingASP.Services.Employees
{
    public class EmployeesService(IUnitOfWork unitOfWork, IPasswordHasher<Employee> passwordHasher) : IEmployeesService
    {
        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync(bool includeInactive)
        {
            var employees = await unitOfWork.EmployeesRepository.GetAllEmployeesAsync(includeInactive);

            return employees
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Role = e.Role,
                Email = e.Email,
                StartDate = e.StartDate,
                IsActive = e.IsActive
            })
            .ToList();
        }

        public async Task<Result<EmployeeResponseDto?>> GetEmployeeByIdAsync(int id)
        {
            var employee = await unitOfWork.EmployeesRepository.GetEmployeeByIdAsync(id);

            if (employee is null)
            {
                return Error.NotFound("Employee not found.");
            }

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Role = employee.Role,
                Email = employee.Email,
                StartDate = employee.StartDate,
                IsActive = employee.IsActive
            };
        }

        public async Task<Result<EmployeeResponseDto>> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var email = employeeDto.Email.Trim().ToLowerInvariant();

            var emailExists = await unitOfWork.EmployeesRepository
            .EmailExistsAsync(email);

            if (emailExists)
            {
                return Error.Conflict("Email is already registered.");
            }


            var newEmployee = new Employee
            {
                Name = employeeDto.Name.Trim(),
                Email = email,
                StartDate = employeeDto.StartDate,
                Role = employeeDto.Role,
                IsActive = true
            };

            newEmployee.PasswordHash = passwordHasher.HashPassword(
                newEmployee,
                employeeDto.Password
            );

            unitOfWork.EmployeesRepository.AddEmployee(newEmployee);
            await unitOfWork.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = newEmployee.Id,
                Name = newEmployee.Name,
                Email = newEmployee.Email,
                Role = newEmployee.Role,
                StartDate = newEmployee.StartDate,
                IsActive = newEmployee.IsActive
            };

        }

        public async Task<Result> UpdateEmployeeAsync(int id, CreateEmployeeDto employeeDto)
        {
            var employee = await unitOfWork.EmployeesRepository.GetEmployeeByIdAsync(id);

            if (employee is null)
            {
                return Error.NotFound("Employee not found.");
            }

            if (!employee.IsActive)
            {
                return Error.Conflict("Cannot update an inactive employee.");
            }

            employee.Name = employeeDto.Name.Trim();
            employee.StartDate = employeeDto.StartDate;
            employee.Role = employeeDto.Role;
            employee.Email = employeeDto.Email;
            employee.PasswordHash = employeeDto.Password;

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeactivateEmployeeAsync(int id)
        {
            var employee = await unitOfWork.EmployeesRepository.GetEmployeeByIdAsync(id);

            if (employee is null)
                return Error.NotFound("Employee not found.");

            employee.IsActive = false;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
