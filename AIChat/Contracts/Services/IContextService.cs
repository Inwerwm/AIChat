using AIChat.Core.Models.ChatGpt;
using AIChat.Models;

namespace AIChat.Contracts.Services;
public interface IContextService
{
    HttpClient HttpClient
    {
        get;
    }

    List<ChatContext> ChatGptContexts
    {
        get;
    }

    ChatContext LastChatGptContext
    {
        get;
        set;
    }

    bool IsOpenAIApiKeyNotSet
    {
        get;
    }

    ChatContext CreateChatContext(string name);
}
