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



            var employeeId = Guid.Parse(GetUserIdFromToken());
            var employee = await _employeeService.GetEmployeeByID(employeeId);

            if (employee == null)
                return BadRequest("Employee not found.");

            var fileName = await SavePdfDocument(saveExpenseRequestDto.ExpenseDocument);
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
            var adminCheckResult = await CompanyAdminControll();
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
            var employeeId = Guid.Parse(GetUserIdFromToken());
            var companyAdminCheck = await CompanyAdminControll();

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
                var adminCheckResult = await CompanyAdminControll();
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
                var adminCheckResult = await CompanyAdminControll();
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


        private string GetUserIdFromToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> CompanyAdminControll()
        {
            var adminId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(adminId))
            {
                return false;
            }

            var adminCompany = await _companyService.GetCompanyByEmployeeId(Guid.Parse(adminId));
            if (adminCompany == null || adminCompany.AdminID != Guid.Parse(adminId))
            {
                return false;

            }
            return true;
        }


        private async Task<string> SavePdfDocument(string base64Pdf)
        {

            if (string.IsNullOrWhiteSpace(base64Pdf))
                return null;


            const string pdfPrefix = "data:application/pdf;base64,";
            if (base64Pdf.StartsWith(pdfPrefix))
            {
                base64Pdf = base64Pdf.Substring(pdfPrefix.Length);
            }
            else
            {
                return null;
            }

            try
            {

                byte[] pdfBytes = Convert.FromBase64String(base64Pdf);


                using (var memoryStream = new MemoryStream(pdfBytes))
                {

                    if (!IsValidPdf(memoryStream))
                        return null;


                    var fileName = Guid.NewGuid().ToString() + ".pdf";
                    var filePath = Path.Combine(StoragePath, fileName);


                    Directory.CreateDirectory(StoragePath);


                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                    return fileName;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool IsValidPdf(Stream pdfStream)
        {
            try
            {

                using (PdfDocument document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.ReadOnly))
                {
                    return document.PageCount > 0;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}

