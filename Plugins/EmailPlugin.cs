
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Azure_Semantic_Kernel_Workshop
{
  public class EmailPlugin 
  {
    private readonly IEmailService _emailService;

    public EmailPlugin(IEmailService emailService)
    {
      _emailService = emailService;
    }

    [KernelFunction("SendEmail")]
    [Description("Send an email")]
    public async Task SendEmail(
        [Description("The subject of the email")] string subject,
        [Description("The body of the email")] string body)
    {
      await _emailService.SendEmailAsync(subject, body);
    }
  }
}
