using BAL.EmailServices;
using DAL.Core.IConfiguration;
using DAL.Models;

namespace BAL.ExpenseServices
{
    public class ExpenseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailService _emailService;


        public ExpenseRequestService(IUnitOfWork unitOfWork, EmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;

        }


        public async Task<ExpenseRequest> CreateExpenseRequest(ExpenseRequest expenseRequest, Guid employeeId, Guid companyId ,string fileName)
        {
            expenseRequest.EmployeeId = employeeId;
            expenseRequest.CompanyId = companyId;
            expenseRequest.Status = "Pending";
            expenseRequest.ExpenseDocument = fileName;

            await _unitOfWork.ExpenseRequests.AddAsync(expenseRequest);
            await _unitOfWork.CommitAsync();

            return expenseRequest;
        }


        public async Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByEmployeeId(Guid employeeId)
        {
            var result = await _unitOfWork.ExpenseRequests.GetExpenseRequestsByEmployeeIdAsync(employeeId);
            return result;

        }


        public async Task<IEnumerable<ExpenseRequest>> GetExpenseRequestsByCompanyId(Guid companyId)
        {
            var result = await _unitOfWork.ExpenseRequests.GetExpenseRequestsByCompanyIdAsync(companyId);
            return result;
        }

        public async Task<ExpenseRequest> GetExpenseRequestById(Guid ExpenseRequestId)
        {
            var result = await _unitOfWork.ExpenseRequests.GetByIdAsync(ExpenseRequestId);
            return result;
        }


        public async Task<ExpenseRequest> ApproveExpenseRequest(Guid expenseRequestId)
        {
            var expenseRequest = await _unitOfWork.ExpenseRequests.GetByIdAsync(expenseRequestId);
            if (expenseRequest == null || !expenseRequest.IsActive)
            {
                throw new InvalidOperationException("Expense request is invalid or already processed.");
            }

            expenseRequest.Status = "Approved";

            expenseRequest.IsActive = false;

            await _unitOfWork.CommitAsync();

            await _emailService.SendEmailAsync(
                expenseRequest.Employee.Email,
                "Expense Request Approved",
                $"Your expense request for {expenseRequest.Name} has been approved."
                );

            return expenseRequest;

        }


        public async Task RejectExpenseRequest(Guid expenseRequestId)
        {
            var expenseRequest = await _unitOfWork.ExpenseRequests.GetByIdAsync(expenseRequestId);
            if (expenseRequest == null || !expenseRequest.IsActive)
            {
                throw new InvalidOperationException("Expense request is invalid or already processed.");
            }

            expenseRequest.Status = "Rejected";
            expenseRequest.IsActive = false;

            await _unitOfWork.CommitAsync();

            await _emailService.SendEmailAsync(
                expenseRequest.Employee.Email,
                "Expense Request Rejected",
                $"Your expense request for {expenseRequest.Name} has been rejected."
                );

        }





    }
}
