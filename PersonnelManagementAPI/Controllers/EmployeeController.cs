using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        private readonly IConfiguration _configuration; 
        private readonly SignInManager<Employee> _signInManager;
        private readonly CompanyService _companyService;

        public EmployeeController(EmployeeService employeeService, IMapper mapper, UserManager<Employee> userManager, IConfiguration configuration, SignInManager<Employee> signInManager,CompanyService companyService)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _companyService = companyService;
        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] SaveEmployeeDTO saveEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _mapper.Map<Employee>(saveEmployeeDto);
            var password = saveEmployeeDto.Password;

            var result = await _employeeService.CreateEmployee(employee, password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { EmployeeId = employeeDTO.EmployeeId }, employeeDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _userManager.FindByEmailAsync(loginDto.Email);
            if (employee == null)
                return Unauthorized(new { success = false, message = "Invalid Credentials" });

            var result = await _userManager.CheckPasswordAsync(employee, loginDto.Password);
            if (!result)
                return Unauthorized(new { success = false, message = "Invalid Credentials" });


            var token = await GenerateJwtToken(employee);
            return Ok(new
            {
                success = true,
                message = "Login successful",
                employeeId = employee.Id,
                token = token 
            });
        }
 
        [HttpPost("{EmployeeId}")]
        public async Task<IActionResult> AddEmployeeDetail(Guid EmployeeId, [FromBody] SaveEmployeeDetailDTO saveEmployeeDetailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeService.GetEmployeeByID(EmployeeId);
            if (employee == null)
                return NotFound();

            var employeeDetail = _mapper.Map<EmployeeDetail>(saveEmployeeDetailDto);
            employeeDetail.EmployeeId = EmployeeId; 

     
            await _employeeService.AddEmployeeDetail(employeeDetail);
            return Ok();
        }


        [HttpPut("{EmployeeId}")]
        public async Task<IActionResult> UpdateEmployeeDetail(Guid EmployeeId, [FromBody] SaveEmployeeDetailDTO saveEmployeeDetailDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeService.GetEmployeeByID(EmployeeId);
            if (employee == null)
                return NotFound();

            var employeeDetail = await _employeeService.GetEmployeeDetailByID(EmployeeId);
            if (employeeDetail == null)
                return NotFound();

            var updatedEmployeeDetail = _mapper.Map<EmployeeDetail>(saveEmployeeDetailDto);
            updatedEmployeeDetail.EmployeeId = EmployeeId; 

            await _employeeService.UpdateEmployeeDetail(employeeDetail, updatedEmployeeDetail);
            return NoContent();
        }

        [Authorize(Roles = "SystemAdministrator")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            var employeeDTOs = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            return Ok(employeeDTOs);
        }


        [HttpGet("{EmployeeId}")]
        [Authorize]
        public async Task<IActionResult> GetEmployeeById(Guid EmployeeId)
        {
            // Get the user ID from the token
            var requestUserId = Guid.Parse(GetUserIdFromToken());

            // Check if the requesting user is the same as the employee
            if (EmployeeId == requestUserId)
            {
                var employee = await _employeeService.GetEmployeeByID(EmployeeId);
                if (employee == null)
                    return NotFound();

                // Map and return employee DTO if the user is the same
                var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
                return Ok(employeeDTO);
            }
            else
            {
                // If not the same, get the employee's company information
                var employee = await _employeeService.GetEmployeeByID(EmployeeId);
                if (employee == null)
                    return NotFound();

                // Check if the employee belongs to a company
                if (employee.CompanyId != null)
                {
                    // Get the company associated with the employee
                    var company = await _companyService.GetCompanyByEmployeeId(EmployeeId);

                    // Check if the requesting user is the admin of that company
                    if (company.AdminID == requestUserId)
                    {
                        var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
                        return Ok(employeeDTO);
                    }
                    else
                    {
                        // If the user is not the admin, return Unauthorized
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
        }



        [HttpPut("{EmployeeId}")]
        public async Task<IActionResult> UpdateEmployee(Guid EmployeeId, [FromBody] SaveEmployeeDTO saveEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeToBeUpdated = await _userManager.FindByIdAsync(EmployeeId.ToString());
            if (employeeToBeUpdated == null)
                return NotFound();

            _mapper.Map(saveEmployeeDto, employeeToBeUpdated);
            var updateResult = await _employeeService.UpdateEmployee(employeeToBeUpdated);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return NoContent();
        }




        [HttpDelete("{EmployeeId}")]
        public async Task<IActionResult> DeleteEmployee(Guid EmployeeId)
        {
            var employee = await _userManager.FindByIdAsync(EmployeeId.ToString());
            if (employee == null)
                return NotFound();

            var deleteResult = await _employeeService.DeleteEmployee(employee);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(deleteResult.Errors);
            }

            return NoContent();
        }

        private async Task<string> GenerateJwtToken(Employee employee)
        {
            var userRoles = await _userManager.GetRolesAsync(employee);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, employee.UserName),
        new Claim("exp", DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])).ToString()) // Add expiration claim
    };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
