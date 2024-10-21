using Data.Models;

namespace DAL.Core.IRepository
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetExpensesByCompanyIdAsync(Guid companyId);
    }
}
