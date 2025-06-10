using System.ComponentModel;
using Microsoft.SemanticKernel;
using SimpleFeedReader;

namespace Azure_Semantic_Kernel_Workshop
{
  public class NewsPlugin
  {
    private readonly INewsService _newsService;
    private readonly ILogger<NewsPlugin> _logger;

    public NewsPlugin(INewsService newsService, ILogger<NewsPlugin> logger)
    {
      _newsService = newsService;
      _logger = logger;
    }

    [KernelFunction("GetNews")]
    [Description("Retrieves a list of the latest news articles matching the specified search query. Returns up to the specified number of articles.")]
    [return: Description("List of news articles")]
    public async Task<List<FeedItem>> GetNewsAsync([Description("Search term to find relevant news articles")]string query, int count = 10)
    {
      _logger.LogInformation("Retrieving news articles for query: {Query}, count: {Count}", query, count);
      var articles = await _newsService.GetNewsAsync(query, count);
      _logger.LogInformation("Successfully retrieved {ArticleCount} news articles for query: {Query}", articles.Count, query);
      return articles;
    }
  }
}
