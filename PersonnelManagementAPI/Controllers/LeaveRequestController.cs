using AutoMapper;
using BAL.CompanyServices;
using BAL.EmployeeServices;
using BAL.LeaveServices;
using DAL.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagementAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]/[action]")]
[ApiController]
public class LeaveRequestController : ControllerBase
{
    private readonly LeaveRequestService _leaveRequestService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly EmployeeService _employeeService;
    private readonly CompanyService _companyService;
    private readonly LeaveService _leaveService;


    public LeaveRequestController(LeaveRequestService leaveRequestService, IMapper mapper, EmployeeService employeeService, IConfiguration configuration, CompanyService companyService, LeaveService leaveService)
    {
        _leaveRequestService = leaveRequestService;
        _mapper = mapper;
        _employeeService = employeeService;
        _configuration = configuration;
        _companyService = companyService;
        _leaveService = leaveService;
        _leaveService = leaveService;
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] SaveLeaveRequestDTO leaveRequestDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (leaveRequestDTO.EndDate < leaveRequestDTO.StartDate)
        {
            ModelState.AddModelError("DateError", "End date must be after start date.");
            return BadRequest(ModelState);
        }

        var employeeId = Guid.Parse(GetUserIdFromToken());
        var employee = await _employeeService.GetEmployeeByID(employeeId);
        var remainDayControl = (leaveRequestDTO.EndDate - leaveRequestDTO.StartDate).Days + 1 > employee.EmployeeDetail.RemainingLeaveDays;

        if (remainDayControl)
        {

            ModelState.AddModelError("DateError", "Not enough leave days remaining.");
            return BadRequest(ModelState);
        }

        var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestDTO);
        var createdLeaveRequest = await _leaveRequestService.CreateLeaveRequest(leaveRequest, employeeId, employee.CompanyId);

        var result = _mapper.Map<LeaveRequestDTO>(createdLeaveRequest);
        return Ok(result);
    }

    [HttpGet("{employeeId}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetLeaveRequestsByEmployeeId(Guid employeeId)
    {
        var leaveRequests = await _leaveRequestService.GetLeaveRequestByEmployeeId(employeeId);

        if (leaveRequests == null || !leaveRequests.Any())
        {
            return NotFound("No leave requests found for this employee.");
        }

        var leaveRequestDTOs = _mapper.Map<IEnumerable<LeaveRequestDTO>>(leaveRequests);
        return Ok(leaveRequestDTOs);
    }

    [HttpGet("{companyId}")]
    [Authorize(Roles = "CompanyAdministrator")]
    public async Task<IActionResult> GetLeaveRequestsByCompanyId(Guid companyId)
    {
        var adminCheckResult = await CompanyAdminControll();
        if (!adminCheckResult)
        {
            return Unauthorized("You are not authorized to do this action.");
        }

        var leaveRequests = await _leaveRequestService.GetLeaveRequestsByCompanyId(companyId);

        if (leaveRequests == null || !leaveRequests.Any())
        {
            return NotFound("No leave requests found for this company.");
        }

        var leaveRequestDTOs = _mapper.Map<IEnumerable<LeaveRequestDTO>>(leaveRequests);
        return Ok(leaveRequestDTOs);
    }

    [HttpPut("{leaveRequestId}")]
    [Authorize(Roles = "CompanyAdministrator")]
    public async Task<IActionResult> ApproveLeaveRequest(Guid leaveRequestId)
    {
        try
        {
            var adminCheckResult = await CompanyAdminControll();
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }

           
            var approvedLeaveRequest = await _leaveRequestService.ApproveLeaveRequest(leaveRequestId);

            var leave  = _mapper.Map<Leave>(approvedLeaveRequest);
            var createdLeave = await _leaveService.CreateLeave(leave, approvedLeaveRequest.EmployeeId, approvedLeaveRequest.CompanyId.Value);

            return Ok(new { Message = "Leave request approved and leave created successfully.", Leave = createdLeave });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [HttpPut("{leaveRequestId}")]
    [Authorize(Roles = "CompanyAdministrator")]
    public async Task<IActionResult> RejectLeaveRequest(Guid leaveRequestId)
    {
        try
        {
            var adminCheckResult = await CompanyAdminControll();
            if (!adminCheckResult)
            {
                return Unauthorized("You are not authorized to do this action.");
            }

            await _leaveRequestService.RejectLeaveRequest(leaveRequestId);
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
}
