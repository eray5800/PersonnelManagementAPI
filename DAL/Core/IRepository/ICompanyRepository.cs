using DAL.Models;
using Data.Models.HelperModels;

namespace DAL.Core.IRepository
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company> GetCompanyByEmployeeId(Guid EmployeeId);
        Task<CompanyDashboardStats> GetCompanyDashboardStats(Guid companyId);

        Task<CompanyDashboardStats> GetEmployeeDashboardStats(Guid employeeId);
    }
}
