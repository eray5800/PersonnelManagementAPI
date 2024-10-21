using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core.IRepository;
using DAL.Models;
using Data.Models.HelperModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(PersonnelManagementDBContext context) : base(context) { }

        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;

        public async Task<Company> GetCompanyByEmployeeId(Guid EmployeeId)
        {

            var result = await PersonnelManagementDBContext.Companies
                                 .Include(c => c.Employees).
                                 SingleOrDefaultAsync(c => c.Employees.Any(e => e.Id == EmployeeId));
            if (result == null)
            {
                result = new Company();
            }

            return result;
        }
        public async Task<CompanyDashboardStats> GetCompanyDashboardStats(Guid companyId)
        {
            var currentDate = DateTime.Now;
            var startOfYear = new DateTime(currentDate.Year, 1, 1); // Start of the current year
            var endOfYear = new DateTime(currentDate.Year, 12, 31); // End of the current year

            var expenseCount = await PersonnelManagementDBContext.Expenses
                                        .Where(e => e.CompanyId == companyId && e.Date >= startOfYear && e.Date <= endOfYear)
                                        .CountAsync();

            var totalExpenses = await PersonnelManagementDBContext.Expenses
                                        .Where(e => e.CompanyId == companyId && e.Date >= startOfYear && e.Date <= endOfYear)
                                        .SumAsync(e => e.Amount);

            var leaveRequestCount = await PersonnelManagementDBContext.LeaveRequests
                                            .Where(lr => lr.CompanyId == companyId && lr.IsActive == true)
                                            .CountAsync();

            var expenseRequestCount = await PersonnelManagementDBContext.ExpenseRequests
                                                .Where(er => er.CompanyId == companyId && er.IsActive == true)
                                                .CountAsync();

            return new CompanyDashboardStats
            {
                ExpenseCount = expenseCount,
                TotalExpenses = totalExpenses,
                LeaveRequestCount = leaveRequestCount,
                ExpenseRequestCount = expenseRequestCount
            };
        }

        public async Task<CompanyDashboardStats> GetEmployeeDashboardStats(Guid employeeId)
        {
            var currentDate = DateTime.Now;
            var startOfYear = new DateTime(currentDate.Year, 1, 1); 
            var endOfYear = new DateTime(currentDate.Year, 12, 31); 

            var expenseCount = await PersonnelManagementDBContext.Expenses
                                        .Where(e => e.EmployeeId == employeeId && e.Date >= startOfYear && e.Date <= endOfYear)
                                        .CountAsync();

            var totalExpenses = await PersonnelManagementDBContext.Expenses
                                        .Where(e => e.EmployeeId == employeeId && e.Date >= startOfYear && e.Date <= endOfYear)
                                        .SumAsync(e => e.Amount);

            var leaveRequestCount = await PersonnelManagementDBContext.LeaveRequests
                                            .Where(lr => lr.EmployeeId == employeeId && lr.IsActive == true)
                                            .CountAsync();

            var expenseRequestCount = await PersonnelManagementDBContext.ExpenseRequests
                                                .Where(er => er.EmployeeId == employeeId && er.IsActive == true)
                                                .CountAsync();

            return new CompanyDashboardStats
            {
                ExpenseCount = expenseCount,
                TotalExpenses = totalExpenses,
                LeaveRequestCount = leaveRequestCount,
                ExpenseRequestCount = expenseRequestCount
            };
        }

    }

}
