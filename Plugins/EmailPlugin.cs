using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Azure_Semantic_Kernel_Workshop
{
  public class EmailPlugin 
  {
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailPlugin> _logger;

    public EmailPlugin(IEmailService emailService, ILogger<EmailPlugin> logger)
    {
      _emailService = emailService;
      _logger = logger;
    }

    [KernelFunction("SendEmail")]
    [Description("Send an email to the user")]
    public async Task SendEmail(
        [Description("The subject of the email")] string subject,
        [Description("The body of the email, this should be in a nice html format with clean styling")] string body)
    {
      _logger.LogInformation("Sending email with subject: {Subject}", subject);
      try
      {
        await _emailService.SendEmailAsync(subject, body);
        _logger.LogInformation("Email sent successfully with subject: {Subject}", subject);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to send email with subject: {Subject}", subject);
        throw;
      }
    }
  }
}
