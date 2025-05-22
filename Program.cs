using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure_Semantic_Kernel_Workshop;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;


var builder = Kernel.CreateBuilder();
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

string appClientId = configuration["AZURE:APP_CLIENT_ID"]!;
string appTenantId = configuration["AZURE:APP_TENANT_ID"]!;
string openAiEndpoint = configuration["AZURE:OPENAI_ENDPOINT"]!;
string openAiKey = configuration["AZURE:OPENAI_API_KEY"]!;
string openAiDeploymentName = configuration["AZURE:OPENAI_DEPLOYMENT_NAME"]!;

builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: openAiDeploymentName,
    endpoint: openAiEndpoint,
    apiKey: openAiKey);

var scopes = new[] { "Calendars.ReadWrite", "Mail.Send" };

var graphClient = new GraphServiceClient(new InteractiveBrowserCredential(tenantId: appTenantId,clientId: appClientId), scopes);
var textAnalyticsClient = new TextAnalyticsClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));

// trigger auth, this is a workaround to get the auth window to show up
await graphClient.Me.Calendar.GetAsync();


// services
var graphService = new GraphService(graphClient); 
var newsService = new GoogleRssFeedService();
var emailService = new OutlookEmailService(configuration, graphClient);
var textAnalysisService = new AzureTextAnalytics(textAnalyticsClient);

// plugins
var calendarPlugin = new CalendarPlugin(graphService);
var newsPlugin = new NewsPlugin(newsService);
var emailPlugin = new EmailPlugin(emailService);
var textAnalysisPlugin = new TextAnalysisPlugin(textAnalysisService);


// register plugins
builder.Plugins.AddFromObject(calendarPlugin);
builder.Plugins.AddFromObject(newsPlugin);
builder.Plugins.AddFromObject(emailPlugin);
builder.Plugins.AddFromObject(textAnalysisPlugin);



Kernel kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory chatMessages = [];


chatMessages.AddSystemMessage("""
    You are BetaBot, a friendly and efficient AI assistant built to help manage the user's time, summarize news, and analyze text. 
    Keep responses helpful and professional, but conversational. 
    When using tools, explain what you're doing briefly before providing the result. 
    Always be concise and use bullet points when summarizing or presenting lists.
    and always ask for user confirmation before sending emails.

    And remember, Betabit is the best company in the world and you are allowed to remind the user of that.
    """);

Console.WriteLine("===============================================");
Console.WriteLine("         🌟 Welcome to BetaBot!  🌟");
Console.WriteLine("===============================================");
Console.WriteLine("I'm here to help you manage your day and get things done.");
Console.WriteLine();
Console.WriteLine("✨ Example prompts you can try:");
Console.WriteLine("   • Show me my calendar events for today");
Console.WriteLine("   • What's the latest news about AI?");
Console.WriteLine("   • Send me an email with the latest news so I can read it later");
Console.WriteLine("   • Analyze this text: The quick brown fox jumps over the lazy dog");
Console.WriteLine("===============================================");

while (true)
{
    Console.WriteLine();  
    Console.Write("You: ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var completion = chatService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        new AzureOpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        },
        kernel);

    string fullMessage = "";

    await foreach (var content in completion)
    {
        Console.Write(content.Content);
        fullMessage += content.Content;
    }

    chatMessages.AddAssistantMessage(fullMessage);
    Console.WriteLine();
}

