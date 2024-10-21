using BAL.EmailServices;
using DAL.Core.IConfiguration;
using DAL.Models;

namespace BAL.LeaveServices
{
    public class LeaveRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly EmailService emailService;


        public LeaveRequestService(IUnitOfWork unitOfWork, EmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
        }


        public async Task<LeaveRequest> CreateLeaveRequest(LeaveRequest leaveRequest, Guid employeeId, Guid? companyId)
        {
            leaveRequest.Status = "Pending";
            leaveRequest.EmployeeId = employeeId;
            leaveRequest.CompanyId = companyId;
            await unitOfWork.LeaveRequests.AddAsync(leaveRequest);
            await unitOfWork.CommitAsync();
            return leaveRequest;
        }

        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestByEmployeeId(Guid employeeId)
        {
            var result = await unitOfWork.LeaveRequests.GetLeaveRequestsByEmployeeId(employeeId);
            return result;
        }

        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByCompanyId(Guid companyId)
        {
            var result = await unitOfWork.LeaveRequests.GetLeaveRequestsByCompanyId(companyId);
            return result;
        }


        public async Task<LeaveRequest> ApproveLeaveRequest(Guid leaveRequestId)
        {
            var leaveRequest = await unitOfWork.LeaveRequests.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null || !leaveRequest.IsActive)
            {
                throw new InvalidOperationException("Leave request is invalid or already processed.");
            }

            if(leaveRequest.TotalDays > leaveRequest.Employee.EmployeeDetail.RemainingLeaveDays)
            {
                throw new InvalidOperationException("Not enough leave days remaining.");
            }

            leaveRequest.Status = "Approved";
            leaveRequest.IsActive = false;
            

            await unitOfWork.CommitAsync();

            // Send approval email
            await emailService.SendEmailAsync(
                leaveRequest.Employee.Email,
                "Leave Request Approved",
                $"Your leave request for {leaveRequest.LeaveType} has been approved."
            );
            return leaveRequest;
        }

        public async Task RejectLeaveRequest(Guid leaveRequestId)
        {
            var leaveRequest = await unitOfWork.LeaveRequests.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null || !leaveRequest.IsActive)
            {
                throw new InvalidOperationException("Leave request is invalid or already processed.");
            }

            leaveRequest.IsActive = false;
            leaveRequest.Status = "Rejected"; // Optionally set the status

            await unitOfWork.CommitAsync();

            // Send rejection email
            await emailService.SendEmailAsync(
                leaveRequest.Employee.Email,
                "Leave Request Rejected",
                $"Your leave request for {leaveRequest.LeaveType} has been rejected."
            );
        }
    }
}
