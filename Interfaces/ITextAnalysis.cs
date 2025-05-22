namespace Azure_Semantic_Kernel_Workshop
{
  public interface ITextAnalysisService
  {
    Task<string> AnalyzeSentimentAsync(string text);
    Task<string> ExtractKeyPhrasesAsync(string text);
    Task<string> DetectLanguageAsync(string text);
  } 
}
