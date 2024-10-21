using DAL.Core;
using DAL.Core.IConfiguration;
using DAL.Core.IRepository;
using DAL.Core.Repository;

namespace DAL.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PersonnelManagementDBContext context;

        private EmployeeRepository employeeRepository;

        private CompanyRequestRepository companyRequestRepository;

        private CompanyRepository companyRepository;

        private LeaveRequestRepository leaveRequestRepository;

        private LeaveRepository leaveRepository;

        private ExpenseRequestRepository expenseRequestRepository;

        private ExpenseRepository expenseRepository;

        private EventRepository eventRepository;

        public IEmployeeRepository Employees => employeeRepository = employeeRepository ?? new EmployeeRepository(context);
        public ICompanyRequestRepository CompanyRequests => companyRequestRepository = companyRequestRepository ?? new CompanyRequestRepository(context);

        public ICompanyRepository Companies => companyRepository = companyRepository ?? new CompanyRepository(context);
        public ILeaveRequestRepository LeaveRequests => leaveRequestRepository = leaveRequestRepository ?? new LeaveRequestRepository(context);
        public ILeaveRepository Leaves => leaveRepository = leaveRepository ?? new LeaveRepository(context);
        public IExpenseRequestRepository ExpenseRequests => expenseRequestRepository = expenseRequestRepository ?? new ExpenseRequestRepository(context);
        public IExpenseRepository Expenses => expenseRepository = expenseRepository ?? new ExpenseRepository(context);

        public IEventRepository Events => eventRepository = eventRepository ?? new EventRepository(context);



        public UnitOfWork(PersonnelManagementDBContext context)
        {
            this.context = context;
        }
        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
