using AutoMapper;
using BAL.EmployeeServices;
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
    public class CompanyRequestController : ControllerBase
    {
        private readonly CompanyRequestService _companyRequestService;
        private readonly EmployeeService _employeeService; 
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private const string StoragePath = "Storage/CompanyRequestDocuments"; 

        public CompanyRequestController(CompanyRequestService companyRequestService, EmployeeService employeeService, IMapper mapper, IConfiguration configuration)
        {
            _companyRequestService = companyRequestService;
            _employeeService = employeeService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("{employeeId}")]
        [Authorize]
        public async Task<IActionResult> GetCompanyRequestByEmployeeId(Guid employeeId)
        {
            var companyRequest = await _companyRequestService.GetCompanyRequestByEmployeeId(employeeId);
            if (companyRequest == null)
                return Ok(new CompanyRequest());

            var companyRequestDto = _mapper.Map<CompanyRequestDTO>(companyRequest);
            return Ok(companyRequestDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCompanyRequest([FromBody] SaveCompanyRequestDTO saveCompanyRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var fileName = await SavePdfDocument(saveCompanyRequestDto.CompanyDocument);
            if (fileName == null)
                return BadRequest("Uploaded file is not a valid PDF document.");


            var employeeId = GetUserIdFromToken();
            if (employeeId == null)
                return Unauthorized("Employee not found in token.");

            var employee = await _employeeService.GetEmployeeByID(Guid.Parse(employeeId));
            if (employee == null)
                return NotFound("Employee not found.");

            var companyRequest = _mapper.Map<CompanyRequest>(saveCompanyRequestDto);
            companyRequest.CompanyDocument = fileName; 
            companyRequest.EmployeeId = employee.Id; 
            companyRequest.Employee = employee; 

            var result = await _companyRequestService.CreateCompanyRequest(companyRequest);
            return CreatedAtAction(nameof(GetCompanyRequestById), new { companyId = result.CompanyId }, result);
        }

        [HttpGet("{companyId}")]
        [Authorize]
        public async Task<IActionResult> GetCompanyRequestById(Guid companyId)
        {
            var companyRequest = await _companyRequestService.GetCompanyRequestById(companyId);
            if (companyRequest == null)
                return NotFound();

            var companyRequestDto = _mapper.Map<CompanyRequestDTO>(companyRequest);
            return Ok(companyRequestDto);
        }

        [HttpGet]
        [Authorize(Roles = "SystemAdministrator")]
        public  IActionResult GetAllCompanyRequests()
        {
            var companyRequests = _companyRequestService.GetAllCompanyRequests();
            var companyRequestDtos = _mapper.Map<IEnumerable<CompanyRequestDTO>>(companyRequests);
            return Ok(companyRequestDtos);
        }

        [HttpDelete("{companyId}")]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> DeleteCompanyRequest(Guid companyId)
        {
            var companyRequest = await _companyRequestService.GetCompanyRequestById(companyId);
            if (companyRequest == null)
                return NotFound();

            var filePath = Path.Combine(StoragePath, companyRequest.CompanyDocument);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await _companyRequestService.DeleteCompanyRequest(companyId);
            return NoContent();
        }

        [HttpGet("{companyId}")]
        [Authorize(Roles = "SystemAdministrator")]
        public IActionResult GetCompanyDocument(Guid companyId)
        {

            var companyRequest = _companyRequestService.GetCompanyRequestById(companyId).Result;
            if (companyRequest == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(StoragePath, companyRequest.CompanyDocument);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("PDF document not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", companyRequest.CompanyDocument);
        }

        [HttpPost("{companyId}")]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> ApproveCompanyRequest(Guid companyId)
        {
            try
            {
                var company = await _companyRequestService.ApproveCompanyRequest(companyId);
                return Ok(company); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{companyId}")]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> RejectCompanyRequest(Guid companyId)
        {
            try
            {
                await _companyRequestService.RejectCompanyRequest(companyId);
                return NoContent(); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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
    }
}
