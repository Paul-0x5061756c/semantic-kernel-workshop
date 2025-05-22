using Azure.AI.TextAnalytics;

namespace Azure_Semantic_Kernel_Workshop
{
    public class AzureTextAnalytics : ITextAnalysisService
    {
        private TextAnalyticsClient _client;

        public AzureTextAnalytics(TextAnalyticsClient client)
        {
            _client = client;
        }

        public async Task<string> AnalyzeSentimentAsync(string text)
        {
            var response = await _client.AnalyzeSentimentAsync(text);
            return response.Value.Sentiment.ToString();
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            var response = await _client.DetectLanguageAsync(text);
            return response.Value.Iso6391Name;
        }

        public async Task<string> ExtractKeyPhrasesAsync(string text)
        {
            var response = await _client.ExtractKeyPhrasesAsync(text);
            return string.Join(", ", response.Value);
        }
    }
}
