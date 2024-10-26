using BAL.CompanyServices;
using Microsoft.Extensions.Configuration;

namespace PersonnelManagementAPI.Helpers
{
    public static  class CompanyHelper
    {
        public static async Task<bool> CompanyAdminControllAsync(HttpContext httpContext,IConfiguration _configuration,CompanyService _companyService )
        {
            var adminId = TokenHelper.GetUserIdFromToken(httpContext, _configuration);
            if (Guid.Empty == adminId)
            {
                return false;
            }

            var adminCompany = await _companyService.GetCompanyByEmployeeId(adminId);
            if (adminCompany == null || adminCompany.AdminID != adminId)
            {
                return false;

            }
            return true;
        }
    }
}
