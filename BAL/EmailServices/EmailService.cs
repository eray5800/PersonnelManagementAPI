using Microsoft.Extensions.Configuration;
using Mailjet.Client;
using Mailjet.Client.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BAL.EmailServices
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var apiKey = _configuration["EmailSettings:ApiKey"];
            var apiSecret = _configuration["EmailSettings:ApiSecret"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            var client = new MailjetClient(apiKey, apiSecret);

            var request = new MailjetRequest
            {
                Resource = Send.Resource
            }
                .Property(Send.FromEmail, fromEmail)
                .Property(Send.FromName, "PMA System Admin")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, body)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", toEmail}
                    }
                });

            var response = await client.PostAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response);
            }
            else
            {
                var errorMessage = response;
            }
        }
    }
}
