using DAL.Models;
using Data.Models;

namespace DAL.Core.IRepository
{
    public interface IExpenseRequestRepository : IGenericRepository<ExpenseRequest>
    {
        Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByEmployeeIdAsync(Guid employeeId);

        Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByCompanyIdAsync(Guid companyId);
    }
}
