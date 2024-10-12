using DAL.Models;

namespace Data.Models
{
    public class CompanyRequest : Base
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public Guid EmployeeId { get; set; }

        public Employee Employee { get; set; }
        public string CompanyDocument { get; set; }

        public string PhoneNumber { get; set; }
    }
}
