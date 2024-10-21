using DAL.Core.IRepository;
using DAL.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core.Repository
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(PersonnelManagementDBContext context) : base(context) { }




        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;

        public override async Task<LeaveRequest> GetByIdAsync(Guid id)
        {
            return await PersonnelManagementDBContext.LeaveRequests.Include(lr => lr.Employee)
                                                                   .Include(lr => lr.Employee.EmployeeDetail)
                                                                   .Where(lr => lr.LeaveRequestId == id)
                                                                   .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeId(Guid employeeId)
        {
            return await PersonnelManagementDBContext.LeaveRequests.Where(x => x.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByCompanyId(Guid companyId)
        {
            return await PersonnelManagementDBContext.LeaveRequests
                .Include(x => x.Employee)
                .Where(x => x.CompanyId == companyId && x.IsActive == true)
                .ToListAsync();
        }
    }
}
