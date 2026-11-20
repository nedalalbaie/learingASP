using learingASP.Enums;
using System.ComponentModel.DataAnnotations;

namespace learingASP.Dtos.Employees
{
    public class CreateEmployeeDto
    {
        [Required]
        public required string Name { get; init; } 

        public required DateOnly StartDate { get; init; }

        [EnumDataType(typeof(Role))]
        public required Role Role { get; init; }

        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(8)]
        public required string Password { get; init; }
    }
}
