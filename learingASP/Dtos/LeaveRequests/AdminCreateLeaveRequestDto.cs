using System.ComponentModel.DataAnnotations;

namespace learingASP.Dtos.LeaveRequests
{
    public class AdminCreateLeaveRequestDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
