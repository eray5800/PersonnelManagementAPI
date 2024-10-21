using BAL.EmailServices;
using BAL.EmployeeServices;
using DAL.Core.IConfiguration;
using Data.Models;

namespace BAL.LeaveServices
{
    public class LeaveService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly EmployeeService employeeService;

        public LeaveService(IUnitOfWork unitOfWork,EmployeeService employeeService)
        {
            this.unitOfWork = unitOfWork;
            this.employeeService = employeeService;
        }


        public async Task<Leave> CreateLeave(Leave leave,Guid employeeId,Guid companyId)
        {
            
            leave.EmployeeId = employeeId;
            leave.CompanyId = companyId;

            var employee = await employeeService.GetEmployeeByID(employeeId);
            employee.EmployeeDetail.RemainingLeaveDays -= leave.TotalDays;

            

            await unitOfWork.Leaves.AddAsync(leave);

            await unitOfWork.CommitAsync();
            return leave;
        }


        public async Task<IEnumerable<Leave>> GetLeavesByCompanyId(Guid companyId)
        {
            var result = await unitOfWork.Leaves.GetLeavesByCompanyIdAsync(companyId);
            return result;
        }
    }
}
