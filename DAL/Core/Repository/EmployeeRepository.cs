using DAL.Core.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(PersonnelManagementDBContext context) : base(context) { }

        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;
        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await PersonnelManagementDBContext.Employees
                .Include(e => e.EmployeeDetail)
                .Include(e => e.EmployeeDetail.Certifications)
                .Include(e => e.EmployeeDetail.Experiences)
                .Include(e => e.EmployeeDetail.Educations)
                .ToListAsync();
        }

        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            return await PersonnelManagementDBContext.Employees
                .Include(e => e.EmployeeDetail)
                .Include(e => e.EmployeeDetail.Certifications)
                .Include(e => e.EmployeeDetail.Experiences)
                .Include(e => e.EmployeeDetail.Educations)
                .FirstOrDefaultAsync(e => e.Id == id);
        }


        public async Task<EmployeeDetail> GetDetailByIdAsync(Guid id)
        {
            return await PersonnelManagementDBContext.EmployeeDetails
                .Include(e => e.Certifications)
                .Include(e => e.Experiences)
                .Include(e => e.Educations)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }


        public async Task AddDetailAsync(EmployeeDetail employeeDetail)
        {
            await PersonnelManagementDBContext.EmployeeDetails.AddAsync(employeeDetail);
        }

        
    }
}