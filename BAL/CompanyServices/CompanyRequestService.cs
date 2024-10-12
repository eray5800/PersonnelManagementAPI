using BAL.RoleServices;
using DAL.Core.IConfiguration;
using DAL.Models;
using Data.Models;
using Microsoft.AspNetCore.Identity;

public class CompanyRequestService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<Employee> userManager;
    private readonly RoleService roleService;

    public CompanyRequestService(IUnitOfWork unitOfWork, UserManager<Employee> userManager,RoleService roleService)
    {
        this.unitOfWork = unitOfWork;
        this.userManager = userManager;
        this.roleService = roleService;
    }

    public async Task<CompanyRequest> CreateCompanyRequest(CompanyRequest companyRequest)
    {
        await unitOfWork.CompanyRequests.AddAsync(companyRequest);
        await unitOfWork.CommitAsync();
        return companyRequest;
    }

    public async Task<CompanyRequest> GetCompanyRequestById(Guid companyId)
    {
        var result = await unitOfWork.CompanyRequests.GetByIdAsync(companyId);
        return result;
    }

    public IEnumerable<CompanyRequest> GetAllCompanyRequests()
    {
        var results =  unitOfWork.CompanyRequests.Find(x => x.IsActive == true);
        return results;
    }

    public async Task<CompanyRequest> GetCompanyRequestByEmployeeId(Guid employeeId)
    {
        var result = await unitOfWork.CompanyRequests.GetCustomerCompanyRequestsById(employeeId);
        return result;
    }

    public async Task DeleteCompanyRequest(Guid companyId)
    {
        var companyRequest = await GetCompanyRequestById(companyId);
        if (companyRequest != null)
        {
            unitOfWork.CompanyRequests.Remove(companyRequest);
            await unitOfWork.CommitAsync();
        }
    }

    public async Task<Company> ApproveCompanyRequest(Guid companyId)
    {

        var companyRequest = await GetCompanyRequestById(companyId);
        if (companyRequest == null || !companyRequest.IsActive)
        {
            throw new InvalidOperationException("Request is invalid or already processed.");
        }


        var newCompany = new Company
        {
            CompanyId = Guid.NewGuid(), 
            Name = companyRequest.CompanyName,
            Address = companyRequest.Address,
            Email = companyRequest.Email,
            PhoneNumber = companyRequest.PhoneNumber,
            AdminID = companyRequest.EmployeeId 
        };

        await unitOfWork.Companies.AddAsync(newCompany);

        await unitOfWork.CommitAsync();


        var companyAdmin = await unitOfWork.Employees.SingleOrDefaultDefaultAsync(e => e.Id == companyRequest.EmployeeId);

        if (companyAdmin != null)
        {

            companyAdmin.CompanyId = newCompany.CompanyId;


            await roleService.AssignRoleAsync(companyAdmin.Id.ToString(), "CompanyAdministrator");


            await userManager.UpdateAsync(companyAdmin); 
        }


        companyRequest.IsActive = false;


        await unitOfWork.CommitAsync();

        return newCompany;
    }


    public async Task RejectCompanyRequest(Guid companyId)
    {
        var companyRequest = await GetCompanyRequestById(companyId);
        if (companyRequest == null)
        {
            throw new InvalidOperationException("Request not found.");
        }

        var filePath = Path.Combine("Storage/CompanyRequestDocuments", companyRequest.CompanyDocument);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        unitOfWork.CompanyRequests.Remove(companyRequest);
        await unitOfWork.CommitAsync();
    }

}
