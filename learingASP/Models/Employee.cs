using learingASP.Enums;

namespace learingASP.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; } = true;
        public List<LeaveRequest> LeaveRequests { get; set; } = [];

       
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
