using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace Azure_Semantic_Kernel_Workshop
{
  public class TextAnalysisPlugin 
  {
    private readonly ITextAnalysisService _textAnalysisService;
    private readonly ILogger<TextAnalysisPlugin> _logger;

    public TextAnalysisPlugin(ITextAnalysisService textAnalysisService, ILogger<TextAnalysisPlugin> logger)
    {
      _textAnalysisService = textAnalysisService;
      _logger = logger;
    }    [KernelFunction("AnalyzeSentiment")]
    [Description("Analyzes the sentiment of the provided text and returns the sentiment score.")]
    [return: Description("Sentiment score")]
    public async Task<string> AnalyzeSentimentAsync([Description("Text to analyze for sentiment")] string text)
    {
      _logger.LogInformation("Analyzing sentiment for text of length: {TextLength}", text.Length);
      var result = await _textAnalysisService.AnalyzeSentimentAsync(text);
      _logger.LogInformation("Sentiment analysis completed with result: {Result}", result);
      return result;
    }

    [KernelFunction("ExtractKeyPhrases")]
    [Description("Extracts key phrases from the provided text and returns them as a list.")]
    [return: Description("List of key phrases")]
    public async Task<string> ExtractKeyPhrasesAsync([Description("Text to extract key phrases from")] string text)
    {
      _logger.LogInformation("Extracting key phrases for text of length: {TextLength}", text.Length);
      var result = await _textAnalysisService.ExtractKeyPhrasesAsync(text);
      _logger.LogInformation("Key phrase extraction completed");
      return result;
    }

    [KernelFunction("DetectLanguage")]
    [Description("Detects the language of the provided text and returns the language code.")]
    [return: Description("Language code")]
    public async Task<string> DetectLanguageAsync([Description("Text to detect language for")] string text)
    {
      _logger.LogInformation("Detecting language for text of length: {TextLength}", text.Length);
      var result = await _textAnalysisService.DetectLanguageAsync(text);
      _logger.LogInformation("Language detection completed with result: {Result}", result);
      return result;
    }

  }
}
