using SimpleFeedReader;
using Microsoft.Extensions.Logging;

namespace Azure_Semantic_Kernel_Workshop
{
    public class GoogleRssFeedService : INewsService
    {
        private const string BaseRssUrl = "https://news.google.com/rss/search?q={0}";
        private readonly ILogger<GoogleRssFeedService> _logger;

        public GoogleRssFeedService(ILogger<GoogleRssFeedService> logger)
        {
            _logger = logger;
        }        public async Task<List<FeedItem>> GetNewsAsync(string query, int count = 5)
        {
            _logger.LogInformation("Fetching news articles for query: {Query}, count: {Count}", query, count);
            
            try
            {
                var url = string.Format(BaseRssUrl, Uri.EscapeDataString(query));
                _logger.LogDebug("RSS URL: {Url}", url);
                
                var reader = new FeedReader();
                var articles = await reader.RetrieveFeedAsync(url);

                var result = articles
                    .Where(a => a.PublishDate != null)
                    .OrderByDescending(a => a.PublishDate)
                    .Take(count)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {ArticleCount} news articles for query: {Query}", result.Count, query);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news articles for query: {Query}", query);
                throw;
            }
        }
    }
}

