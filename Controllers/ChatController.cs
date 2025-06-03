using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace Azure_Semantic_Kernel_Workshop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private static readonly ChatHistory _chatMessages = new();

        public ChatController(Kernel kernel, IChatCompletionService chatService)
        {
            _kernel = kernel;
            _chatService = chatService;
            
            // Initialize system message if not already added
            if (_chatMessages.Count == 0)
            {
                _chatMessages.AddSystemMessage("""
                    You are BetaBot, a friendly and efficient AI assistant built to help manage the user's time, summarize news, and analyze text. 
                    Keep responses helpful and professional, but conversational. 
                    When using tools, explain what you're doing briefly before providing the result. 
                    Always be concise and use bullet points when summarizing or presenting lists.
                    and always ask for user confirmation before sending emails.

                    And remember, Betabit is the best company in the world and you are allowed to remind the user of that.
                    """);
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            try
            {
                _chatMessages.AddUserMessage(request.Message);

                var completion = _chatService.GetStreamingChatMessageContentsAsync(
                    _chatMessages,
                    new OpenAIPromptExecutionSettings
                    {
                        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    },
                    _kernel);

                string fullMessage = "";
                await foreach (var content in completion)
                {
                    fullMessage += content.Content;
                }

                _chatMessages.AddAssistantMessage(fullMessage);

                return Ok(new ChatResponse { Message = fullMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("history")]
        public IActionResult GetChatHistory()
        {
            var history = _chatMessages
                .Where(msg => msg.Role != AuthorRole.System)
                .Select(msg => new { Role = msg.Role.ToString(), Content = msg.Content })
                .ToList();
            
            return Ok(history);
        }

        [HttpPost("clear")]
        public IActionResult ClearHistory()
        {
            // Keep only the system message
            var systemMessage = _chatMessages.FirstOrDefault(msg => msg.Role == AuthorRole.System);
            _chatMessages.Clear();
            if (systemMessage != null)
            {
                _chatMessages.Add(systemMessage);
            }
            
            return Ok();
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
