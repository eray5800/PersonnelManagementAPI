using System.ComponentModel.DataAnnotations;

namespace PersonnelManagementAPI.DTO
{
    public class SaveEmployeeDTO
    {
        public string UserName { get; set; }

        
        public string Password { get; set; }
        [EmailAddress]

        public string Email { get; set; }
    }
}
