using System.ComponentModel.DataAnnotations;

namespace learingASP.Dtos.LeaveRequests
{
    public class CreateLeaveRequestDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
