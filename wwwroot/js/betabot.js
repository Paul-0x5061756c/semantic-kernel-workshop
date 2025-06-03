// BetaBot Chat Application - Enhanced functionality
class BetaBotChat {
  constructor() {
    this.messagesContainer = document.getElementById("messages");
    this.messageInput = document.getElementById("messageInput");
    this.sendBtn = document.getElementById("sendBtn");
    this.chatHistory = [];
    this.isTyping = false;

    this.init();
  }

  init() {
    // Auto-resize textarea
    this.messageInput.addEventListener("input", this.autoResize.bind(this));

    // Send message on Enter (but allow Shift+Enter for new lines)
    this.messageInput.addEventListener(
      "keydown",
      this.handleKeydown.bind(this)
    );

    // Load chat history on startup
    this.loadChatHistory();

    // Add typing indicator functionality
    this.setupTypingIndicator();
  }

  autoResize() {
    this.messageInput.style.height = "auto";
    this.messageInput.style.height =
      Math.min(this.messageInput.scrollHeight, 120) + "px";
  }

  handleKeydown(e) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      this.sendMessage();
    }
  }

  setupTypingIndicator() {
    let typingTimer;
    this.messageInput.addEventListener("input", () => {
      if (!this.isTyping) {
        this.isTyping = true;
        // Could add "user is typing" indicator here if needed
      }

      clearTimeout(typingTimer);
      typingTimer = setTimeout(() => {
        this.isTyping = false;
      }, 1000);
    });
  }

  async loadChatHistory() {
    try {
      const response = await fetch("/api/chat/history");
      if (response.ok) {
        const history = await response.json();
        // Clear existing messages except welcome message
        const welcomeMessage =
          this.messagesContainer.querySelector(".message.assistant");
        this.messagesContainer.innerHTML = "";
        if (welcomeMessage) {
          this.messagesContainer.appendChild(welcomeMessage);
        }

        // Add history messages
        history.forEach((msg) => {
          this.addMessage(msg.Content, msg.Role === "User");
        });
      }
    } catch (error) {
      console.error("Error loading chat history:", error);
    }
  }

  addMessage(content, isUser = false) {
    const messageDiv = document.createElement("div");
    messageDiv.className = `message ${isUser ? "user" : "assistant"}`;

    const avatar = document.createElement("div");
    avatar.className = "avatar";
    avatar.textContent = isUser ? "👤" : "🤖";

    const contentDiv = document.createElement("div");
    contentDiv.className = "content";
    contentDiv.innerHTML = this.formatMessage(content);

    messageDiv.appendChild(avatar);
    messageDiv.appendChild(contentDiv);
    this.messagesContainer.appendChild(messageDiv);

    this.scrollToBottom();
    // Store in local history
    this.chatHistory.push({ content, isUser, timestamp: new Date() });
  }

  formatMessage(content) {
    let formatted = content.replace(/\n/g, "<br>");

    // Bullet points
    formatted = formatted.replace(/^[-•*]\s/gm, "• ");

    // Numbered lists
    formatted = formatted.replace(
      /^\d+\.\s/gm,
      (match) => `<strong>${match}</strong>`
    );

    // === STEP 1: Temporarily replace markdown links with placeholders
    const markdownLinkMap = new Map();
    let linkCounter = 0;

    formatted = formatted.replace(
      /\[([^\]]+)\]\((https?:\/\/[^\)]+)\)/g,
      (match, text, url) => {
        const placeholder = `__MARKDOWN_LINK_${linkCounter++}__`;
        markdownLinkMap.set(
          placeholder,
          `<a href="${url}" target="_blank" style="color: #667eea; text-decoration: none; border-bottom: 1px dotted #667eea;">${text}</a>`
        );
        return placeholder;
      }
    );

    // Bold text
    formatted = formatted.replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>");

    // Inline code
    formatted = formatted.replace(
      /`(.*?)`/g,
      '<code style="background: #f1f3f4; padding: 2px 4px; border-radius: 3px;">$1</code>'
    );

    // Standalone URLs (now safely)
    formatted = formatted.replace(
      /(https?:\/\/[^\s<]+)(?![^<]*<\/a>)/g,
      '<a href="$1" target="_blank" style="color: #667eea; text-decoration: none; border-bottom: 1px dotted #667eea;">$1</a>'
    );

    // Emails
    formatted = formatted.replace(
      /([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})/g,
      '<a href="mailto:$1" style="color: #667eea; text-decoration: none; border-bottom: 1px dotted #667eea;">$1</a>'
    );

    // === STEP 2: Replace placeholders back with real markdown links
    for (const [placeholder, linkHtml] of markdownLinkMap) {
      formatted = formatted.replace(placeholder, linkHtml);
    }

    return formatted;
  }

  scrollToBottom() {
    this.messagesContainer.scrollTop = this.messagesContainer.scrollHeight;
  }

  showLoading(message = "BetaBot is thinking...") {
    const loadingDiv = document.createElement("div");
    loadingDiv.className = "loading";
    loadingDiv.id = "loading";
    loadingDiv.innerHTML = `<div class="spinner"></div>${message}`;
    this.messagesContainer.appendChild(loadingDiv);
    this.scrollToBottom();
  }

  hideLoading() {
    const loading = document.getElementById("loading");
    if (loading) {
      loading.remove();
    }
  }

  getLoadingMessage(userMessage) {
    const msg = userMessage.toLowerCase();

    if (
      msg.includes("calendar") ||
      msg.includes("event") ||
      msg.includes("meeting") ||
      msg.includes("appointment")
    ) {
      return "📅 Checking your calendar...";
    } else if (
      msg.includes("email") ||
      msg.includes("send") ||
      msg.includes("mail")
    ) {
      return "📧 Preparing your email...";
    } else if (
      msg.includes("news") ||
      msg.includes("latest") ||
      msg.includes("headlines")
    ) {
      return "📰 Fetching the latest news...";
    } else if (
      msg.includes("analyze") ||
      msg.includes("sentiment") ||
      msg.includes("language") ||
      msg.includes("phrases") ||
      msg.includes("detect")
    ) {
      return "🔍 Analyzing text...";
    } else if (
      msg.includes("time") ||
      msg.includes("date") ||
      msg.includes("today")
    ) {
      return "⏰ Getting current time...";
    } else {
      return "🤖 BetaBot is thinking...";
    }
  }

  async sendMessage() {
    const message = this.messageInput.value.trim();
    if (!message) return;

    // Disable input and button
    this.sendBtn.disabled = true;
    this.messageInput.disabled = true;

    // Add user message
    this.addMessage(message, true);
    this.messageInput.value = "";
    this.messageInput.style.height = "auto";

    // Show loading with smart message
    const loadingMessage = this.getLoadingMessage(message);
    this.showLoading(loadingMessage);

    try {
      const response = await fetch("/api/chat/send", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ message: message }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      this.hideLoading();
      this.addMessage(data.message, false);
    } catch (error) {
      this.hideLoading();
      this.addMessage(
        "Sorry, I encountered an error. Please try again. If the problem persists, please check your connection and try again.",
        false
      );
      console.error("Error:", error);
    } finally {
      // Re-enable input and button
      this.sendBtn.disabled = false;
      this.messageInput.disabled = false;
      this.messageInput.focus();
    }
  }

  setMessage(message) {
    this.messageInput.value = message;
    this.messageInput.focus();
    this.autoResize();
  }

  async clearChat() {
    if (confirm("Are you sure you want to clear the chat history?")) {
      try {
        await fetch("/api/chat/clear", {
          method: "POST",
        });

        // Clear messages except the initial greeting
        this.messagesContainer.innerHTML = `
                    <div class="message assistant">
                        <div class="avatar">🤖</div>
                        <div class="content">
                            Hello! I'm BetaBot, your AI assistant from Betabit (the best company in the world! 😉). 
                            I can help you manage your calendar, get the latest news, analyze text, and send emails. 
                            What would you like to do today?
                        </div>
                    </div>
                `;

        // Clear local history
        this.chatHistory = [];
      } catch (error) {
        console.error("Error clearing chat:", error);
        alert("Failed to clear chat. Please try again.");
      }
    }
  }

  // Export chat history as text
  exportChatHistory() {
    if (this.chatHistory.length === 0) {
      alert("No chat history to export.");
      return;
    }

    const exportText = this.chatHistory
      .map(
        (msg) =>
          `[${msg.timestamp.toLocaleString()}] ${
            msg.isUser ? "You" : "BetaBot"
          }: ${msg.content}`
      )
      .join("\n\n");

    const blob = new Blob([exportText], { type: "text/plain" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `betabot-chat-${new Date().toISOString().split("T")[0]}.txt`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
}

// Initialize the chat application
let betaBotChat;

window.addEventListener('load', () => {
    betaBotChat = new BetaBotChat();
    
    // Expose functions for HTML onclick events
    window.sendMessage = () => betaBotChat.sendMessage();
    window.setMessage = (message) => betaBotChat.setMessage(message);
    window.clearChat = () => betaBotChat.clearChat();
    window.exportChat = () => betaBotChat.exportChatHistory();
});
