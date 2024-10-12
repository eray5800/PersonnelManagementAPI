using System.ComponentModel.DataAnnotations;

namespace PersonnelManagementAPI.DTO
{
    public class SaveAdminEmployeeDTO
    {
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
