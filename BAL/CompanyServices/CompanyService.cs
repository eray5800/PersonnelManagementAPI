using BAL.CompanyServices.Helpers;
using BAL.EmailServices;
using BAL.RoleServices;
using DAL.Core.IConfiguration;
using DAL.Models;
using Data.Models.HelperModels;
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

        public async Task<CompanyDashboardStats> GetCompanyAdminStats(Guid companyId)
        {
            var result = await unitOfWork.Companies.GetCompanyDashboardStats(companyId);
            return result;
        }

        public async Task<CompanyDashboardStats> GetEmployeeDashboardStats(Guid employeeId)
        {
            var result = await unitOfWork.Companies.GetEmployeeDashboardStats(employeeId);
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

        public async Task<IdentityResult> FireEmployeeAsync(Guid employeeId, Guid companyId)
        {
            // Get the employee by their ID
            var employee = await userManager.FindByIdAsync(employeeId.ToString());

            if (employee == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Employee not found"
                });
            }

           

            // Check if the employee belongs to the same company
            if (employee.CompanyId != companyId)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Employee does not belong to this company"
                });
            }

            // Remove the employee from the company (optional: if you want to just deactivate or remove them from company)
            employee.CompanyId = null;

            // Delete the employee from the system
            var result = await userManager.DeleteAsync(employee);

            return result;
        }

    }
}
