using DAL.Core.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class LeaveRepository : GenericRepository<Leave>, ILeaveRepository
    {
        public LeaveRepository(PersonnelManagementDBContext context) : base(context) { }


        public async Task<IEnumerable<Leave>> GetLeavesByCompanyIdAsync(Guid companyId)
        {
            return await PersonnelManagementDBContext.Leaves
                .Include(l => l.Employee)
                .Where(l => l.CompanyId == companyId)
                .ToListAsync();
        }

        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;
    }
}
