using DAL.Models;

namespace PersonnelManagementAPI.DTO
{
    public class LeaveDTO
    {
        public Guid LeaveId { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalDays => (EndDate - StartDate).Days + 1;
        public string LeaveType { get; set; }

        public string Status { get; set; }


        public string Reason { get; set; }

    }
}
