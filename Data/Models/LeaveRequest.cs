using DAL.Enums;

namespace DAL.Models
{
    public class LeaveRequest : Base
    {
        public Guid LeaveRequestId { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Company Company { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalDays => (EndDate - StartDate).Days + 1;
        public LeaveType LeaveType { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected
        public string Reason { get; set; }
    }


}
