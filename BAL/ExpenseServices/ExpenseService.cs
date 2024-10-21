using DAL.Core.IConfiguration;
using Data.Models;
using Microsoft.Extensions.Configuration;

namespace BAL.ExpenseServices
{
    public class ExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;


        public ExpenseService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Expense> CreateExpense(Expense expense,Guid employeeId,Guid companyId)
        {
            expense.EmployeeId = employeeId;
            expense.CompanyId = companyId;

            await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.CommitAsync();
            return expense;

        }


        public async Task<IEnumerable<Expense>> GetExpensesByCompanyId(Guid companyId)
        {
            var result = await _unitOfWork.Expenses.GetExpensesByCompanyIdAsync(companyId);
            return result;

        }

        public async Task<Expense> GetExpenseById(Guid expenseId)
        {
            var result = await _unitOfWork.Expenses.GetByIdAsync(expenseId);
            return result;

        }
    }
}
