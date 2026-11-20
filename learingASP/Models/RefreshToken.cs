namespace learingASP.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
