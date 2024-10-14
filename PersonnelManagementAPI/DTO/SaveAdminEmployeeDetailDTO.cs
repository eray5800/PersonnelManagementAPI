using DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace PersonnelManagementAPI.DTO
{
    public class SaveAdminEmployeeDetailDTO
    {
        public string Address { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string City { get; set; }

        public int RemainingLeaveDays { get; set; }


        public DateTime BirthDate { get; set; }
        [AllowNull]
        public IEnumerable<Education>? Educations { get; set; }
        [AllowNull]

        public IEnumerable<Certification>? Certifications { get; set; }
        [AllowNull]

        public IEnumerable<Experience>? Experiences { get; set; }
    }
}
