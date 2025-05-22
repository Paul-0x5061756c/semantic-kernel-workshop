using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Azure_Semantic_Kernel_Workshop
{
  public class TextAnalysisPlugin 
  {
    private readonly ITextAnalysisService _textAnalysisService;

    public TextAnalysisPlugin(ITextAnalysisService textAnalysisService)
    {
      _textAnalysisService = textAnalysisService;
    }

    [KernelFunction("AnalyzeSentiment")]
    [Description("Analyzes the sentiment of the provided text and returns the sentiment score.")]
    [return: Description("Sentiment score")]
    public async Task<string> AnalyzeSentimentAsync([Description("Text to analyze for sentiment")] string text)
    {
      return await _textAnalysisService.AnalyzeSentimentAsync(text);
    }

    [KernelFunction("ExtractKeyPhrases")]
    [Description("Extracts key phrases from the provided text and returns them as a list.")]
    [return: Description("List of key phrases")]
    public async Task<string> ExtractKeyPhrasesAsync([Description("Text to extract key phrases from")] string text)
    {
      return await _textAnalysisService.ExtractKeyPhrasesAsync(text);
    }

    [KernelFunction("DetectLanguage")]
    [Description("Detects the language of the provided text and returns the language code.")]
    [return: Description("Language code")]
    public async Task<string> DetectLanguageAsync([Description("Text to detect language for")] string text)
    {
      return await _textAnalysisService.DetectLanguageAsync(text);
    }

  }
}
