using SimpleFeedReader;

namespace Azure_Semantic_Kernel_Workshop
{
  public interface INewsService 
  {
    Task<List<FeedItem>>GetNewsAsync(string query, int count = 10);
    Task<string>GetItemContentAsync(string url);
  }
}
