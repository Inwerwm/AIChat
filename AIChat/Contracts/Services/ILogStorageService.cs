using AIChat.Core.Models.ChatGpt;

namespace AIChat.Contracts.Services;
internal interface ILogStorageService
{
    Task SaveContextAsync(ChatGptContext context);

    IAsyncEnumerable<ChatGptContext> LoadContextsAsync(HttpClient httpClient);

    void RemoveContext(Guid contextId);
}
