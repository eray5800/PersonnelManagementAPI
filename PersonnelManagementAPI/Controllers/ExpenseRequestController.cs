using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using BAL.ExpenseServices;
using DAL.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PersonnelManagementAPI.DTO;
using PersonnelManagementAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonnelManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExpenseRequestController : ControllerBase
    {

        private readonly ExpenseRequestService _expenseRequestService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly EmployeeService _employeeService;
        private readonly CompanyService _companyService;
        private readonly ExpenseService _expenseService;
        private const string StoragePath = "Storage/ExpenseDocuments";



        public ExpenseRequestController(ExpenseRequestService expenseRequestService, IMapper mapper, IConfiguration configuration, EmployeeService employeeService, CompanyService companyService, ExpenseService expenseService)
        {
            _expenseRequestService = expenseRequestService;
            _mapper = mapper;
            _configuration = configuration;
            _employeeService = employeeService;
            _companyService = companyService;
            _expenseService = expenseService;
        }



        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateExpenseRequest([FromBody] SaveExpenseRequestDTO saveExpenseRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var employeeId = TokenHelper.GetUserIdFromToken(HttpContext,_configuration);
            var employee = await _employeeService.GetEmployeeByID(employeeId);

            if (employee == null)
                return BadRequest("Employee not found.");

            var fileName = await PdfDocumentHelper.SavePdfDocument(saveExpenseRequestDto.ExpenseDocument,StoragePath);
            if (fileName == null)
                return BadRequest("Uploaded file is not a valid PDF document.");

            var expenseRequest = _mapper.Map<ExpenseRequest>(saveExpenseRequestDto);

            var createdExpenseRequest = await _expenseRequestService.CreateExpenseRequest(expenseRequest, employeeId, employee.CompanyId ?? Guid.NewGuid(), fileName);

            var result = _mapper.Map<ExpenseRequestDTO>(createdExpenseRequest);
            return Ok(result);

        }

        [HttpGet("{employeeId}")]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> GetExpenseRequestsByEmployeeId(Guid employeeId)
        {
            var expenseRequests = await _expenseRequestService.GetExpenseRequestsByEmployeeId(employeeId);

            if (expenseRequests == null || !expenseRequests.Any())
            {
                return NotFound("No expense requests found for this employee.");
            }

            var expenseRequestDTOs = _mapper.Map<IEnumerable<ExpenseRequestDTO>>(expenseRequests);
            return Ok(expenseRequestDTOs);
        }

        [HttpGet("{companyId}")]
        [Authorize(Roles = "CompanyAdministrator")]

        public async Task<IActionResult> GetExpenseRequestsByCompanyId(Guid companyId)
        {
            var adminCheckResult = await CompanyHelper.CompanyAdminControllAsync(HttpContext, _configuration, _companyService);
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }

            var expenseRequests = await _expenseRequestService.GetExpenseRequestsByCompanyId(companyId);

            if (expenseRequests == null || !expenseRequests.Any())
            {
                return NotFound("No expense requests found for this company.");
            }

            var expenseRequestDTOs = _mapper.Map<IEnumerable<ExpenseRequestDTO>>(expenseRequests);
            return Ok(expenseRequestDTOs);
        }


        [HttpGet("{expenseRequestId}")]
        [Authorize(Roles = "CompanyAdministrator,Employee")]

        public async Task<IActionResult> GetExpenseRequestDocument(Guid expenseRequestId)
        {
            var expenseRequest = await _expenseRequestService.GetExpenseRequestById(expenseRequestId);

            if (expenseRequest == null)
            {
                return NotFound("Expense request not found.");
            }
            var employeeId = TokenHelper.GetUserIdFromToken(HttpContext, _configuration);
            var companyAdminCheck = await CompanyHelper.CompanyAdminControllAsync(HttpContext, _configuration, _companyService);

            if (expenseRequest.EmployeeId != employeeId && !companyAdminCheck)
            {
                return Unauthorized("You are not authorized to do this action.");
            }

            var filePath = Path.Combine(StoragePath, expenseRequest.ExpenseDocument);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("PDF document not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", expenseRequest.ExpenseDocument);
        }

        [HttpPut("{expenseRequestId}")]
        [Authorize(Roles = "CompanyAdministrator")]

        public async Task<IActionResult> ApproveExpenseRequest(Guid expenseRequestId)
        {
            try
            {
                var adminCheckResult = await CompanyHelper.CompanyAdminControllAsync(HttpContext,_configuration,_companyService);
                if (!adminCheckResult)
                {
                    return Unauthorized("You are not authorized to do this action.");
                }

                var approvedExpenseRequest = await _expenseRequestService.ApproveExpenseRequest(expenseRequestId);
                var expense = _mapper.Map<Expense>(approvedExpenseRequest);
                var createdExpense = _expenseService.CreateExpense(expense, approvedExpenseRequest.EmployeeId, approvedExpenseRequest.CompanyId.Value);
                return Ok(expense);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{expenseRequestId}")]
        [Authorize(Roles = "CompanyAdministrator")]

        public async Task<IActionResult> RejectExpenseRequest(Guid expenseRequestId)
        {
            try
            {
                var adminCheckResult = await CompanyHelper.CompanyAdminControllAsync(HttpContext,_configuration,_companyService);
                if (!adminCheckResult)
                {
                    return Unauthorized("You are not authorized to do this action.");
                }

                await _expenseRequestService.RejectExpenseRequest(expenseRequestId);
                return Ok(new { Message = "Leave request rejected successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}

