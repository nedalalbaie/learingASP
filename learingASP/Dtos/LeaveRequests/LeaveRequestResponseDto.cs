using learingASP.Enums;

namespace learingASP.Dtos.LeaveRequests
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public decimal Days { get; set; }

        public LeaveStatus Status { get; set; }

        public bool IsAdminCreated { get; set; }

        public int CreatedById { get; set; }

        public int? ReviewedById { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
