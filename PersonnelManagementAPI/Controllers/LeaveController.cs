using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using BAL.LeaveServices;
using Microsoft.AspNetCore.Http;
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
            var adminCheckResult = await CompanyAdminControll();
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }
            var result = await _leaveService.GetLeavesByCompanyId(companyId);

            var leaves = _mapper.Map<IEnumerable<LeaveDTO>>(result);
            return Ok(leaves);
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
    }
}
