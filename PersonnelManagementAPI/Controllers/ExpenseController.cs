using AutoMapper;
using BAL.CompanyServices;
using BAL.ExpenseServices;
using BAL.LeaveServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagementAPI.DTO;
using PersonnelManagementAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonnelManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseService _expenseService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly CompanyService _companyService;
        private const string StoragePath = "Storage/ExpenseDocuments";



        public ExpenseController(IMapper mapper, ExpenseService expenseService, IConfiguration configuration, CompanyService companyService)
        {

            _mapper = mapper;
            _expenseService = expenseService;
            _configuration = configuration;
            _companyService = companyService;
        }



        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetExpensesByCompanyId(Guid companyId)
        {
            var adminCheckResult = await CompanyHelper.CompanyAdminControllAsync(HttpContext, _configuration, _companyService) ;
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }
            var result = await _expenseService.GetExpensesByCompanyId(companyId);

            var expenses = _mapper.Map<IEnumerable<ExpenseDTO>>(result); 
            return Ok(expenses);
        }


        [HttpGet("{expenseRequestId}")]
        [Authorize(Roles = "CompanyAdministrator")]

        public async Task<IActionResult> GetExpenseDocument(Guid expenseRequestId)
        {
            var expense = await _expenseService.GetExpenseById(expenseRequestId);

            if (expense == null)
            {
                return NotFound("Expense not found.");
            }
            var companyAdminCheck = await CompanyHelper.CompanyAdminControllAsync(HttpContext,_configuration,_companyService);

            if (!companyAdminCheck)
            {
                return Unauthorized("You are not authorized to do this action.");
            }

            var filePath = Path.Combine(StoragePath, expense.ExpenseDocument);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("PDF document not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", expense.ExpenseDocument);
        }

    }
}
