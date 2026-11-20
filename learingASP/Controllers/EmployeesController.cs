using learingASP.Common;
using learingASP.Dtos.Employees;
using learingASP.Services.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace learingASP.Controllers
{
    [Route("employees")]
    [ApiController]
    [Authorize]
    public class EmployeesController(IEmployeesService employeesService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EmployeeResponseDto>>> GetEmployees([FromQuery] bool includeInactive = false)
          => Ok(await employeesService.GetAllEmployeesAsync(includeInactive));


        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
        {
            var result = await employeesService.GetEmployeeByIdAsync(id);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(CreateEmployeeDto request)
        {
            var result = await employeesService.CreateEmployeeAsync(request);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return Ok(result.Value);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(
        int id,
        CreateEmployeeDto employeeDto)
        {
            var result = await employeesService.UpdateEmployeeAsync(id, employeeDto);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return NoContent();
        }

        [HttpDelete("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var result = await employeesService.DeactivateEmployeeAsync(id);

            if (!result.IsSuccess)
            {
                return this.ToProblem(result);
            }

            return NoContent();
        }
    }
}
