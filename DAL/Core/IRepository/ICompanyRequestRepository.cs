using Data.Models;

namespace DAL.Core.IRepository
{
    public interface ICompanyRequestRepository : IGenericRepository<CompanyRequest>
    {
        Task<CompanyRequest> GetCustomerCompanyRequestsById(Guid employeeId);
    }
}
