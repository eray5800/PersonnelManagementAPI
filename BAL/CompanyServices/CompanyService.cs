using BAL.CompanyServices.Helpers;
using BAL.EmailServices;
using BAL.RoleServices;
using DAL.Core.IConfiguration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace BAL.CompanyServices
{
    public class CompanyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<Employee> userManager;
        private readonly EmailService emailService;
        private readonly RoleService roleService;

        public CompanyService(IUnitOfWork unitOfWork, UserManager<Employee> userManager, EmailService emailService, RoleService roleService)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.emailService = emailService;
            this.roleService = roleService;
        }

        public async Task<Company> GetCompanyByEmployeeId(Guid employeeId)
        {
            var result = await unitOfWork.Companies.GetCompanyByEmployeeId(employeeId);
            if (result == null)
            {
                result = new Company();
            }

            return result;
        }

        public async Task<IdentityResult> CreateAdminEmployeeAsync(Employee employee, Guid companyId)
        {
            string randomPassword = PasswordHelper.GenerateRandomPassword(12);

            employee.CompanyId = companyId;

            var result = await userManager.CreateAsync(employee, randomPassword);

            if (result.Succeeded)
            {
                await roleService.AssignRoleAsync(employee, "Employee");

                string subject = "Your PMA Employee Account Details";
                string body = $"Dear {employee.UserName},<br/><br/>" +
                              $"Your employee account has been created successfully.<br/>" +
                              $"Email: {employee.Email}<br/>" +
                              $"Password: {randomPassword}<br/><br/>" ;

                await emailService.SendEmailAsync(employee.Email, subject, body);
            }

            return result;
        }
    }
}
