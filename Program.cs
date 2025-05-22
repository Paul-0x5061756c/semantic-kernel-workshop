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

// trigger auth, this is a workaround to get the auth window to show up
await graphClient.Me.Calendar.GetAsync();


// services
var graphService = new GraphService(graphClient); 
var newsService = new GoogleRssFeedService();
var emailService = new OutlookEmailService(configuration, graphClient);

// plugins
var calendarPlugin = new CalendarPlugin(graphService);
var newsPlugin = new NewsPlugin(newsService);
var emailPlugin = new EmailPlugin(emailService);


// register plugins
builder.Plugins.AddFromObject(calendarPlugin);
builder.Plugins.AddFromObject(newsPlugin);
builder.Plugins.AddFromObject(emailPlugin);



Kernel kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory chatMessages = [];

while (true)
{
    Console.Write("Prompt: ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var completion = chatService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        new AzureOpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
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

