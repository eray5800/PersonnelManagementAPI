namespace PersonnelManagementAPI.DTO
{
    public class SaveCompanyRequestDTO
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public string CompanyDocument { get; set; }

        public string PhoneNumber { get; set; }
    }
}
