using DAL.Core.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {

        public ExpenseRepository(PersonnelManagementDBContext context) : base(context) { }

        public async Task<IEnumerable<Expense>> GetExpensesByCompanyIdAsync(Guid companyId) 
        {
            return await PersonnelManagementDBContext.Expenses
                        .Include(e => e.Employee)
                        .Where(e => e.CompanyId == companyId)
                        .ToListAsync();
        }

        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;
    }
}
