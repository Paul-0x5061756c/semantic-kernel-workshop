namespace Azure_Semantic_Kernel_Workshop
{
  public interface IEmailService 
  {
    Task SendEmailAsync(string subject, string body);
  }
}
