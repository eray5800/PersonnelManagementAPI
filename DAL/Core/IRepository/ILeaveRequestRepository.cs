using DAL.Models;

namespace DAL.Core.IRepository
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeId(Guid employeeId);

        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByCompanyId(Guid companyId);
    }
}
