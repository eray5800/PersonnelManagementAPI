using Microsoft.AspNetCore.Identity;
using DAL.Models;
using DAL.Core.IConfiguration;

namespace BAL.EmployeeServices
{
    public class EmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<Employee> userManager;

        public EmployeeService(IUnitOfWork unitOfWork, UserManager<Employee> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateEmployee(Employee employee, string password)
        {
            if (await EmailExists(employee.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });
            }

            var result = await userManager.CreateAsync(employee, password);
            if (!result.Succeeded)
            {
                return result;
            }

            await unitOfWork.CommitAsync();

            return result;
        }

        public async Task<EmployeeDetail> AddEmployeeDetail(EmployeeDetail employeeDetail)
        {
            await unitOfWork.Employees.AddDetailAsync(employeeDetail);
            await unitOfWork.CommitAsync();

            return employeeDetail;
        }



        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            var results = await unitOfWork.Employees.GetAllAsync();
            return results;
        }

        public async Task<Employee> GetEmployeeByID(Guid employeeID)
        {
            var result = await unitOfWork.Employees.GetByIdAsync(employeeID);
            return result;
        }

        public async Task<EmployeeDetail> GetEmployeeDetailByID(Guid employeeID)
        {
            var result = await unitOfWork.Employees.GetDetailByIdAsync(employeeID);
            return result;

        }
        public async Task<IdentityResult> UpdateEmployee(Employee employeeToBeUpdated)
        {
            if (await EmailExists(employeeToBeUpdated.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });
            }
            return await userManager.UpdateAsync(employeeToBeUpdated);
        }

        public async Task<IdentityResult> DeleteEmployee(Employee employee)
        {
            return await userManager.DeleteAsync(employee);
        }



        public async Task<EmployeeDetail> AdminUpdateEmployeeDetail(EmployeeDetail employeeDetailToBeUpdated, EmployeeDetail newEmployeeDetail)
        {
            // Map new details to the existing one
            employeeDetailToBeUpdated.Address = newEmployeeDetail.Address;
            employeeDetailToBeUpdated.Position = newEmployeeDetail.Position;
            employeeDetailToBeUpdated.Department = newEmployeeDetail.Department;
            employeeDetailToBeUpdated.City = newEmployeeDetail.City;
            employeeDetailToBeUpdated.Educations = newEmployeeDetail.Educations;
            employeeDetailToBeUpdated.Certifications = newEmployeeDetail.Certifications;
            employeeDetailToBeUpdated.Experiences = newEmployeeDetail.Experiences;
            employeeDetailToBeUpdated.RemainingLeaveDays = newEmployeeDetail.RemainingLeaveDays;

            // Commit changes to the database
            await unitOfWork.CommitAsync();

            return employeeDetailToBeUpdated;
        }


        public async Task<EmployeeDetail> UpdateEmployeeDetail(EmployeeDetail employeeDetailToBeUpdated, EmployeeDetail newEmployeeDetail)
        {
            // Map new details to the existing one
            employeeDetailToBeUpdated.Address = newEmployeeDetail.Address;
            employeeDetailToBeUpdated.Position = newEmployeeDetail.Position;
            employeeDetailToBeUpdated.Department = newEmployeeDetail.Department;
            employeeDetailToBeUpdated.City = newEmployeeDetail.City;
            employeeDetailToBeUpdated.Educations = newEmployeeDetail.Educations;
            employeeDetailToBeUpdated.Certifications = newEmployeeDetail.Certifications;
            employeeDetailToBeUpdated.Experiences = newEmployeeDetail.Experiences;
            employeeDetailToBeUpdated.BirthDate  = newEmployeeDetail.BirthDate;
            

            // Commit changes to the database
            await unitOfWork.CommitAsync();

            return employeeDetailToBeUpdated;
        }


        private async Task<bool> EmailExists(string email, Guid? employeeId = null)
        {
            // E-postayı kontrol et
            var existingEmployee = await unitOfWork.Employees.GetAllAsync();
            return existingEmployee.Any(e => e.Email == email && (employeeId == null || e.Id != employeeId));
        }

    }
}
