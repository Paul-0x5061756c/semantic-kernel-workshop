using SimpleFeedReader;

namespace Azure_Semantic_Kernel_Workshop
{
    public class GoogleRssFeedService : INewsService
    {
        private const string BaseRssUrl = "https://news.google.com/rss/search?q={0}";

        public Task<string> GetItemContentAsync(string url)
        {
          var httpClient = new HttpClient();
          return httpClient.GetStringAsync(url);
        }

        public async Task<List<FeedItem>> GetNewsAsync(string query, int count = 5)
        {
          var url = string.Format(BaseRssUrl, Uri.EscapeDataString(query));
          var reader = new FeedReader();

          var articles = await reader.RetrieveFeedAsync(url);

          return articles
              .Where(a => a.PublishDate != null)
              .OrderByDescending(a => a.PublishDate)
              .Take(count)
              .ToList();
        }
    }
}

