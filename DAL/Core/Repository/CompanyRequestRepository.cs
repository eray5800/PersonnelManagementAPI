using DAL.Core.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class CompanyRequestRepository : GenericRepository<CompanyRequest>, ICompanyRequestRepository
    {
        public CompanyRequestRepository(PersonnelManagementDBContext context) : base(context) { }


        public async Task<CompanyRequest> GetCustomerCompanyRequestsById(Guid employeeId)
        {

            return await PersonnelManagementDBContext.CompanyRequests
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsActive);
        }
        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;
    }
}
