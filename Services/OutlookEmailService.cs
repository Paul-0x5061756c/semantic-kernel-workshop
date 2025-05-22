using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;

namespace Azure_Semantic_Kernel_Workshop
{
    public class OutlookEmailService : IEmailService
    {
        private readonly string toEmail = string.Empty;
        private readonly GraphServiceClient _graphClient;

        public OutlookEmailService(IConfiguration configuration, GraphServiceClient graphClient)
        {
          toEmail = configuration["TO_EMAIL"] ?? throw new ArgumentNullException("TO_EMAIL"); 
          _graphClient = graphClient;
        }


        public async Task SendEmailAsync(string subject, string body)
        {
            var message = new SendMailPostRequestBody 
            {
                Message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = body
                    },
                    ToRecipients = new List<Recipient>
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = toEmail
                            }
                        }
                    }
                },
                SaveToSentItems = true

            };

            await _graphClient.Me.SendMail.PostAsync(message);
        }
    }
}

