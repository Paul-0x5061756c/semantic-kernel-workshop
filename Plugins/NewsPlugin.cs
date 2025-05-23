using System.ComponentModel;
using Microsoft.SemanticKernel;
using SimpleFeedReader;

namespace Azure_Semantic_Kernel_Workshop
{
  public class NewsPlugin
  {
    private readonly INewsService _newsService;

    public NewsPlugin(INewsService newsService)
    {
      _newsService = newsService;
    }
    [KernelFunction("GetNews")]
    [Description("Retrieves a list of the latest news articles matching the specified search query. Returns up to the specified number of articles.")]
    [return: Description("List of news articles")]
    public async Task<List<FeedItem>> GetNewsAsync([Description("Search term to find relevant news articles")]string query, int count = 10)
    {
      return await _newsService.GetNewsAsync(query, count);
    }
  }
}
