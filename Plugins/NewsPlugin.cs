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

    [KernelFunction("GetNewsContent")]
    [Description("Retrieves the content of a news article given its URL.")]
    [return: Description("Content of the news article")]
    public async Task<string> GetNewsContentAsync([Description("URL of the news article")]string url)
    {
      return await _newsService.GetItemContentAsync(url);
    }
  }
}
