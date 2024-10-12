using AutoMapper;
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

        public EmployeeController(EmployeeService employeeService, IMapper mapper, UserManager<Employee> userManager, IConfiguration configuration, SignInManager<Employee> signInManager)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager; 
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            var employeeDTOs = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            return Ok(employeeDTOs);
        }


        [HttpGet("{EmployeeId}")]
        public async Task<IActionResult> GetEmployeeById(Guid EmployeeId)
        {
            var employee = await _employeeService.GetEmployeeByID(EmployeeId);
            if (employee == null)
                return NotFound();

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return Ok(employeeDTO);
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
    }
}
