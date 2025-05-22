# Semantic Kernel Workshop

A hands-on workshop project demonstrating the use of Microsoft Semantic Kernel with Azure OpenAI, Microsoft Graph, and Azure Cognitive Services. This repo provides a console-based AI assistant (BetaBot) that helps manage your calendar, summarize news, analyze text, and send emails—all powered by modern AI and cloud APIs.

## Features

- **AI Chat Assistant (BetaBot):** Friendly, efficient, and interactive assistant for your daily productivity.
- **Calendar Integration:** View and add events to your Outlook calendar using Microsoft Graph.
- **Email Automation:** Send emails via Outlook with user confirmation.
- **News Summarization:** Fetch and summarize the latest news using Google News RSS feeds.
- **Text Analysis:** Analyze sentiment, extract key phrases, and detect language using Azure Cognitive Services.
- **Plugin Architecture:** Easily extendable with new plugins and services.

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Azure subscription with:
  - Azure OpenAI resource
  - Azure Cognitive Services (Text Analytics)
  - Microsoft Graph API access
- API keys and endpoints for the above services

### Setup
1. **Clone the repository:**
   ```sh
   git clone <this-repo-url>
   cd semantic-kernel-workshop
   ```
2. **Configure settings:**
   - Copy `appsettings.json` and/or create `appsettings.Development.json`.
   - Fill in your Azure and API credentials:
     - `AZURE:APP_CLIENT_ID`
     - `AZURE:APP_TENANT_ID`
     - `AZURE:OPENAI_DEPLOYMENT_NAME`
     - `AZURE:OPENAI_ENDPOINT`
     - `AZURE:OPENAI_API_KEY`
     - `TO_EMAIL` (for email sending)
3. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
4. **Run the application:**
   ```sh
   dotnet run
   ```

## Example Prompts
- Show me my calendar events for today
- What's the latest news about AI?
- Send me an email with the latest news so I can read it later
- Analyze this text: The quick brown fox jumps over the lazy dog

## Project Structure
- `Program.cs` — Main entry point and chat loop
- `Services/` — Integrations for Graph, Email, News, and Text Analytics
- `Plugins/` — Semantic Kernel plugins for calendar, news, email, and text analysis
- `Interfaces/` — Service interfaces for dependency injection
- `appsettings.json` — Configuration file for API keys and endpoints

## Credits
- [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel)
- [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)
- [Microsoft Graph](https://learn.microsoft.com/en-us/graph/overview)
- [Azure Cognitive Services](https://learn.microsoft.com/en-us/azure/cognitive-services/)

---

_BetaBot says: Betabit is the best company in the world!_
