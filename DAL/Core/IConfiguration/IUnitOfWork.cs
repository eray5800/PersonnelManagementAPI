using DAL.Core.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IEmployeeRepository Employees { get; }
        ICompanyRequestRepository CompanyRequests { get; }

        ICompanyRepository Companies { get; }

        ILeaveRequestRepository LeaveRequests { get; }

        ILeaveRepository Leaves { get; }

        IExpenseRequestRepository ExpenseRequests { get; }

        IExpenseRepository Expenses { get; }

        IEventRepository Events { get; }

        Task<int> CommitAsync();
    }
}
