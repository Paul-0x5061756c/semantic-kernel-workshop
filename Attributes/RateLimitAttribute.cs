using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace Azure_Semantic_Kernel_Workshop
{
    public class RateLimitAttribute : Attribute, IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<string, ClientRateLimit> _clients = new();
        private readonly int _requestsPerMinute;

        public RateLimitAttribute(int requestsPerMinute = 100)
        {
            _requestsPerMinute = requestsPerMinute;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var clientId = GetClientIdentifier(context.HttpContext);
            var now = DateTime.UtcNow;

            // Clean up old entries (optional optimization)
            CleanupOldEntries();

            var clientRateLimit = _clients.GetOrAdd(clientId, _ => new ClientRateLimit());

            lock (clientRateLimit)
            {
                // Remove requests older than 1 minute
                clientRateLimit.Requests.RemoveAll(r => (now - r).TotalMinutes >= 1);

                if (clientRateLimit.Requests.Count >= _requestsPerMinute)
                {
                    context.Result = new ObjectResult(new { error = "Rate limit exceeded. Please try again later." })
                    {
                        StatusCode = 429
                    };
                      // Add rate limit headers
                    context.HttpContext.Response.Headers["X-RateLimit-Limit"] = _requestsPerMinute.ToString();
                    context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = "0";
                    context.HttpContext.Response.Headers["X-RateLimit-Reset"] = GetResetTime(clientRateLimit.Requests).ToString();
                    
                    return;
                }

                clientRateLimit.Requests.Add(now);
            }            // Add rate limit headers for successful requests
            var remaining = Math.Max(0, _requestsPerMinute - clientRateLimit.Requests.Count);
            context.HttpContext.Response.Headers["X-RateLimit-Limit"] = _requestsPerMinute.ToString();
            context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            
            if (clientRateLimit.Requests.Count > 0)
            {
                context.HttpContext.Response.Headers["X-RateLimit-Reset"] = GetResetTime(clientRateLimit.Requests).ToString();
            }

            await next();
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Use IP address as client identifier
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            var clientIp = forwarded ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            return clientIp;
        }

        private long GetResetTime(List<DateTime> requests)
        {
            if (requests.Count == 0) return 0;
            
            var oldestRequest = requests.Min();
            var resetTime = oldestRequest.AddMinutes(1);
            return ((DateTimeOffset)resetTime).ToUnixTimeSeconds();
        }

        private void CleanupOldEntries()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-2); // Clean up entries older than 2 minutes
            var keysToRemove = _clients
                .Where(kvp => kvp.Value.Requests.Count == 0 || kvp.Value.Requests.Max() < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _clients.TryRemove(key, out _);
            }
        }

        private class ClientRateLimit
        {
            public List<DateTime> Requests { get; } = new();
        }
    }
}
