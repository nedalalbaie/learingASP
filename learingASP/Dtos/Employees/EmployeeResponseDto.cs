using learingASP.Enums;
using System.ComponentModel.DataAnnotations;

namespace learingASP.Dtos.Employees
{
    public class EmployeeResponseDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateOnly StartDate { get; init; }

        public Role Role { get; init; }
        public bool IsActive { get; init; }

    }
}
