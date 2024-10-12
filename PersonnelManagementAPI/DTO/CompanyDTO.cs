namespace PersonnelManagementAPI.DTO
{
    public class CompanyDTO
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public Guid AdminID { get; set; }

        public IEnumerable<EmployeeDTO> Employees { get; set; }
    }
}
