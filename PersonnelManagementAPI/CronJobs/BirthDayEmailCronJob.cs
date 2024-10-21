using BAL.EmailServices;
using BAL.EmployeeServices;
using Quartz;

namespace PersonnelManagementAPI.CronJobs
{
    public class BirthDayEmailCronJob : IJob
    {
        private readonly EmployeeService _employeeService;
        private readonly EmailService _emailService;
        private readonly ILogger<BirthDayEmailCronJob> _logger;

        public BirthDayEmailCronJob(EmployeeService employeeService, EmailService emailService, ILogger<BirthDayEmailCronJob> logger)
        {
            _employeeService = employeeService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var today = DateTime.Today;

                var employees = await _employeeService.GetAllEmployees();

                var birthdayEmployees = employees
                    .Where(e => e.EmployeeDetail != null &&
                                e.EmployeeDetail.BirthDate.Month == today.Month &&
                                e.EmployeeDetail.BirthDate.Day == today.Day);

                foreach (var employee in birthdayEmployees)
                {
                    var subject = "Happy Birthday!";
                    var body = $"Dear {employee.UserName},<br><br>We wish you a very Happy Birthday!<br><br>Best regards,<br>PMA System Admin";

                    await _emailService.SendEmailAsync(employee.Email, subject, body);
                    _logger.LogInformation($"Birthday email sent to: {employee.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending birthday emails: {ex.Message}");
            }
        }

    }
}
