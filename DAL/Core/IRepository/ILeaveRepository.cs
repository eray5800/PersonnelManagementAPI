using Data.Models;

namespace DAL.Core.IRepository
{
    public interface ILeaveRepository : IGenericRepository<Leave>
    {
        Task<IEnumerable<Leave>> GetLeavesByCompanyIdAsync(Guid companyId);
    }
}
