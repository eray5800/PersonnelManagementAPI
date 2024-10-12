    namespace DAL.Models
    {
        public class EmployeeDetail : Base
        {
            public Guid EmployeeDetailId { get; set; }

            public Guid EmployeeId { get; set; }
            public virtual Employee Employee { get; set; }

            public string Address { get; set; }

            public string Position { get; set; }
            public string Department { get; set; }
            public string City { get; set; }
            public DateTime BirthDate { get; set; }
            public virtual ICollection<Education> Educations  { get; set; }
            public virtual ICollection<Certification> Certifications { get; set; }
            public virtual ICollection<Experience> Experiences { get; set; }

            public int RemainingLeaveDays {  get; set; }
        }
    }
