namespace learingASP.Dtos.LeaveRequests
{
    public class LeaveBalanceDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }

        public decimal Entitlement { get; set; }
        public decimal ApprovedDays { get; set; }
        public decimal PendingDays { get; set; }
        public decimal AvailableDays { get; set; }
    }
}
