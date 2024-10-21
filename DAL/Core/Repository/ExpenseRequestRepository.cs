using DAL.Core.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class ExpenseRequestRepository : GenericRepository<ExpenseRequest>, IExpenseRequestRepository
    {

        public ExpenseRequestRepository(PersonnelManagementDBContext dbContext) : base(dbContext) { }


        public async Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByEmployeeIdAsync(Guid employeeId)
        {

            return await PersonnelManagementDBContext.ExpenseRequests
                .Include(x => x.Employee)
                .Where(er =>er.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByCompanyIdAsync(Guid companyId)
        {
            return await PersonnelManagementDBContext.ExpenseRequests
                .Include(er => er.Employee)
                .Where(er =>er.CompanyId == companyId && er.IsActive == true)
                .ToListAsync();
        }


        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;


    }
}
