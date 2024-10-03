using DAL.Core.IRepository;
using DAL.Models;

namespace DAL.Core.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(PersonnelManagementDBContext context) : base(context) { }

        public Task AddAsync(Employee employee)
        {
            PersonnelManagementDBContext.Employees.Add(employee);
            return Task.CompletedTask;
            
        }

        public Task AddDetailAsync(EmployeeDetail employeeDetail)
        {
            PersonnelManagementDBContext.EmployeeDetails.Add(employeeDetail);
            return Task.CompletedTask;

        }

        private PersonnelManagementDBContext PersonnelManagementDBContext { get { return Context; } }


    }
}
