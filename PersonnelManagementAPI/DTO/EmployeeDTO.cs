using DAL.Models;

namespace PersonnelManagementAPI.DTO
{
    public class EmployeeDTO
    {
        public Guid EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string City { get; set; }

        public string Role { get; set; }

        public DateTime BirthDate {  get; set; }

        public Guid CompanyId { get; set; }
        public IEnumerable<Education> Educations { get; set; }
        public IEnumerable<Certification> Certifications { get; set; }
        public IEnumerable<Experience> Experiences { get; set; }
        public int RemainingLeaveDays { get; set; }
        public IEnumerable<LeaveRequestDTO> LeaveRequests { get; set; }
        public IEnumerable<ExpenseRequestDTO> ExpenseRequests { get; set; }
    }
}
