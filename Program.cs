using Azure.Core;
using Azure.Identity;
using Azure_Semantic_Kernel_Workshop;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;


var builder = Kernel.CreateBuilder();
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

string appClientId = configuration["AZURE:APP_CLIENT_ID"]!;
string appTenantId = configuration["AZURE:APP_TENANT_ID"]!;
string openAiEndpoint = configuration["AZURE:OPENAI_ENDPOINT"]!;
string openAiKey = configuration["AZURE:OPENAI_API_KEY"]!;
string openAiDeploymentName = configuration["AZURE:OPENAI_DEPLOYMENT_NAME"]!;

builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: openAiDeploymentName,
    endpoint: openAiEndpoint,
    apiKey: openAiKey);

var credential = new InteractiveBrowserCredential(
    tenantId : appTenantId,
    clientId : appClientId);
    

await credential.GetTokenAsync(new TokenRequestContext(new[] { "User.Read", "Calendars.ReadWrite" }), CancellationToken.None);
var graphClient = new GraphServiceClient(credential, new[] { "Calendars.ReadWrite" });

var graphService = new GraphService(graphClient); 
var calendarPlugin = new CalendarPlugin(graphService);

builder.Plugins.AddFromObject(calendarPlugin);

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

