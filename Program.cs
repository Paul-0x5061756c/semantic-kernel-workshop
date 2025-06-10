using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure_Semantic_Kernel_Workshop;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuration values
string appClientId = builder.Configuration["AZURE:APP_CLIENT_ID"]!;
string appTenantId = builder.Configuration["AZURE:APP_TENANT_ID"]!;
string openAiEndpoint = builder.Configuration["AZURE:OPENAI_ENDPOINT"]!;
string openAiKey = builder.Configuration["AZURE:OPENAI_API_KEY"]!;
string openAiDeploymentName = builder.Configuration["AZURE:OPENAI_DEPLOYMENT_NAME"]!;

// Add Semantic Kernel
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: openAiDeploymentName,
    endpoint: openAiEndpoint,
    apiKey: openAiKey);

// Set up Graph client and services
var scopes = new[] { "Calendars.ReadWrite", "Mail.Send" };
var graphClient = new GraphServiceClient(new InteractiveBrowserCredential(tenantId: appTenantId, clientId: appClientId), scopes);
var textAnalyticsClient = new TextAnalyticsClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));

// Services
var graphService = new GraphService(graphClient);
var newsService = new GoogleRssFeedService();
var emailService = new OutlookEmailService(builder.Configuration, graphClient);
var textAnalysisService = new AzureTextAnalytics(textAnalyticsClient);

// Plugins
var calendarPlugin = new CalendarPlugin(graphService);
var newsPlugin = new NewsPlugin(newsService);
var emailPlugin = new EmailPlugin(emailService);
var textAnalysisPlugin = new TextAnalysisPlugin(textAnalysisService);

// Register plugins
kernelBuilder.Plugins.AddFromObject(calendarPlugin);
kernelBuilder.Plugins.AddFromObject(newsPlugin);
kernelBuilder.Plugins.AddFromObject(emailPlugin);
kernelBuilder.Plugins.AddFromObject(textAnalysisPlugin);

// Build kernel and register as singleton
var kernel = kernelBuilder.Build();
builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton(kernel.GetRequiredService<IChatCompletionService>());
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Request.Headers.Add("X-Api-Key", builder.Configuration["API_KEY"]);
    }
    await next.Invoke();
});


app.MapControllers();

// Fallback to serve index.html for any non-API routes
app.MapFallbackToFile("index.html");

// Trigger auth on startup
try
{
    await graphClient.Me.Calendar.GetAsync();
    Console.WriteLine("✅ Microsoft Graph authentication successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Microsoft Graph authentication failed: {ex.Message}");
    Console.WriteLine("You may need to authenticate when using calendar or email features.");
}

Console.WriteLine("===============================================");
Console.WriteLine("         🌟 BetaBot Web App Started!  🌟");
Console.WriteLine("===============================================");
Console.WriteLine($"🌐 Open your browser and go to: http://localhost:{app.Urls.FirstOrDefault()?.Split(':').Last() ?? "5000"}");
Console.WriteLine("💡 The web interface is now ready!");
Console.WriteLine("===============================================");

app.Run();

