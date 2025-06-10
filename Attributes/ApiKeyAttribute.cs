using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
  private const string ApiKeyHeaderName = "X-Api-Key";

  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    var expectedApiKey = context.HttpContext.RequestServices
      .GetRequiredService<IConfiguration>()["API_KEY"];

    if (context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey) &&
        apiKey == expectedApiKey)
    {
      await next();
    }
    else
    {
      context.Result = new UnauthorizedResult();
      return;
    }
  }
}
