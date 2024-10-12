using AutoMapper;
using BAL.CompanyServices;
using BAL.RoleServices;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagementAPI.DTO;

namespace PersonnelManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CompanyService _companyService;
        private readonly RoleService _roleService;

        public CompanyController(IMapper mapper, CompanyService companyService, RoleService roleService)
        {
            _mapper = mapper;
            _companyService = companyService;
            _roleService = roleService;
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

    }
}
