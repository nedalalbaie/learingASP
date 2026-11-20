using learingASP.Enums;

namespace learingASP.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }                
        public Employee Employee { get; set; } = null!;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Days { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public bool IsAdminCreated { get; set; }
        public int CreatedById { get; set; }               
        public Employee CreatedBy { get; set; } = null!;

        public int? ReviewedById { get; set; }              
        public Employee? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
