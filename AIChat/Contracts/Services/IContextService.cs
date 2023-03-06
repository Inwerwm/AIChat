using AIChat.Core.Models.ChatGpt;

namespace AIChat.Contracts.Services;
public interface IContextService
{
    HttpClient HttpClient
    {
        get;
    }

    List<ChatGptContext> ChatGptContexts
    {
        get;
    }

    bool IsOpenAIApiKeyNotSet
    {
        get;
    }

    ChatGptContext CreateChatGptContext();
}
