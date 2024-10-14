using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using BAL.RoleServices;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagementAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonnelManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CompanyService _companyService;
        private readonly RoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly EmployeeService _employeeService;

        public CompanyController(IMapper mapper, CompanyService companyService, RoleService roleService, IConfiguration configuration, EmployeeService employeeService)
        {
            _mapper = mapper;
            _companyService = companyService;
            _roleService = roleService;
            _configuration = configuration;
            _employeeService = employeeService;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetCompanyByEmployeeId(Guid employeeId)
        {
            var company = await _companyService.GetCompanyByEmployeeId(employeeId);
            if (company == null)
            {
                return NotFound();
            }

            var companyDTO = _mapper.Map<CompanyDTO>(company);

            foreach (var employee in companyDTO.Employees)
            {
                var roles = await _roleService.GetUserRolesAsync(employee.EmployeeId);
                employee.Role = roles.FirstOrDefault();
            }

            return Ok(companyDTO);
        }

        [HttpPost("{employeeId}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> CreateCompanyAdminEmployee([FromBody] SaveAdminEmployeeDTO adminEmployeeDTO, Guid employeeId)
        {
            var company = await _companyService.GetCompanyByEmployeeId(employeeId);
            if (company == null)
            {
                return NotFound();
            }
            if (company.AdminID != employeeId)
            {
                return Unauthorized();
            }

            var employee = _mapper.Map<Employee>(adminEmployeeDTO);

            var result = await _companyService.CreateAdminEmployeeAsync(employee, company.CompanyId);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "employee created by companyAdmin successfully, and email has been sent with login details." });
        }

        [HttpDelete("{employeeId}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> FireEmployee(Guid employeeId)
        {
            
            var adminId = GetUserIdFromToken(); 
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized();
            }

            var adminCompany = await _companyService.GetCompanyByEmployeeId(Guid.Parse(adminId));

            if (adminCompany == null || adminCompany.AdminID != Guid.Parse(adminId))
            {
                return Unauthorized("You are not authorized to fire employees from this company.");
            }

            if (employeeId == Guid.Parse(adminId))
            {
                return BadRequest("You cannot fire yourself.");
            }


            var result = await _companyService.FireEmployeeAsync(employeeId, adminCompany.CompanyId);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Employee has been fired successfully." });
        }

        [HttpPut("{employeeId}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> AdminEmployeeDetailUpdate(Guid employeeId, [FromBody] SaveAdminEmployeeDetailDTO SaveAdminEmployeeDetailDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized();
            }

            var adminCompany = await _companyService.GetCompanyByEmployeeId(Guid.Parse(adminId));

            if (adminCompany == null || adminCompany.AdminID != Guid.Parse(adminId))
            {
                return Unauthorized("You are not authorized to update employees from this company.");
            }

            var employeeDetailToBeUpdated = await _employeeService.GetEmployeeDetailByID(employeeId);
            if (employeeDetailToBeUpdated == null)
            {
                return NotFound("Employee detail not found.");
            }

            var newEmployeeDetail = _mapper.Map<EmployeeDetail>(SaveAdminEmployeeDetailDTO);

            var updatedEmployeeDetail = await _employeeService.AdminUpdateEmployeeDetail(employeeDetailToBeUpdated, newEmployeeDetail);

            return Ok(updatedEmployeeDetail);
        }

        [HttpPost("{employeeId}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> AdminEmployeeDetailCreate(Guid employeeId, [FromBody] SaveAdminEmployeeDetailDTO saveAdminEmployeeDetailDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized();
            }

            var adminCompany = await _companyService.GetCompanyByEmployeeId(Guid.Parse(adminId));
            if (adminCompany == null || adminCompany.AdminID != Guid.Parse(adminId))
            {
                return Unauthorized("You are not authorized to add details for employees of this company.");
            }

            var employee = await _employeeService.GetEmployeeByID(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            employee.IsActive = true;
            var employeeDetail = _mapper.Map<EmployeeDetail>(saveAdminEmployeeDetailDTO);
            employeeDetail.EmployeeId = employeeId;

            await _employeeService.AddEmployeeDetail(employeeDetail);

            return Ok(new { Message = "Employee detail created successfully." });
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
