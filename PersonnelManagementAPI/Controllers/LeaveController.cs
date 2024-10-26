using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using BAL.LeaveServices;
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
    public class LeaveController : ControllerBase
    {

        private readonly LeaveService _leaveService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly CompanyService _companyService;


        public LeaveController(IMapper mapper,LeaveService leaveService, IConfiguration configuration,CompanyService companyService)
        {

            _mapper = mapper;
            _leaveService = leaveService;
            _configuration = configuration;
            _companyService = companyService;
        }



        [HttpGet("{companyId}")]

        public async Task<IActionResult> GetLeavesByCompanyId(Guid companyId)
        {
            var adminCheckResult = await CompanyHelper.CompanyAdminControllAsync(HttpContext,_configuration,_companyService);
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }
            var result = await _leaveService.GetLeavesByCompanyId(companyId);

            var leaves = _mapper.Map<IEnumerable<LeaveDTO>>(result);
            return Ok(leaves);
        }


    }
}
