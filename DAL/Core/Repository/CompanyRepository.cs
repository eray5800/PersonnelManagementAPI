using DAL.Core.IRepository;
using DAL.Models;
using Data.Models;
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
            if(result == null)
            {
                result = new Company();
            }

            return result;
        }}


    }

