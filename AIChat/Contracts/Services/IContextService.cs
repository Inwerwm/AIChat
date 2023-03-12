using AIChat.Models;

namespace AIChat.Contracts.Services;
public interface IContextService
{
    HttpClient HttpClient
    {
        get;
    }

    Task<IReadOnlyList<ChatContext>> GetChatGptContextsAsync();

    Task<ChatContext> GetLastChatGptContextAsync();
    void SetLastChatGptContext(ChatContext value);

    bool IsOpenAIApiKeyNotSet
    {
        get;
    }

    ChatContext CreateChatContext(string name);

    void AddChatContext(ChatContext chatContext);

    void RemoveChatContext(ChatContext chatContext);

    void SaveChatContext(ChatContext chatContext);
}
