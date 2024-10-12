namespace PersonnelManagementAPI.DTO
{
    public class LeaveRequestDTO
    {
        public Guid LeaveRequestId { get; set; } 
        public Guid EmployeeId { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public int TotalDays { get; set; } 
        public string LeaveType { get; set; } 
        public string Status { get; set; }
        public string Reason { get; set; } 
    }
}