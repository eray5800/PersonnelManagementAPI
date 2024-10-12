using DAL.Models;

namespace DAL.Core.IRepository
{
    public interface ICompanyRepository :  IGenericRepository<Company>
    {
        Task<Company> GetCompanyByEmployeeId(Guid EmployeeId);
    }
}
