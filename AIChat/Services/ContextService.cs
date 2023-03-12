using AIChat.Contracts.Services;
using AIChat.Core.Models.ChatGpt;
using AIChat.Models;

namespace AIChat.Services;
internal class ContextService : IContextService
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogStorageService _logStorageService;
    private List<ChatContext>? _chatGptContexts;
    private ChatContext? lastChatGptContext;

    public HttpClient HttpClient
    {
        get;
    }

    private Action<ChatContext, string> SaveNameFunction => async (sender, name) =>
    {
        if (sender.Context.MessageLog.Any())
        {
            await _logStorageService.SaveContextAsync(sender.Context);
        }
    };

    public async Task<IReadOnlyList<ChatContext>> GetChatGptContextsAsync()
    {
        // 初回読み込み時は設定ファイルから読み込む
        _chatGptContexts ??= await _logStorageService.LoadContextsAsync(HttpClient).Select(cgc => new ChatContext(cgc.Name ?? "new chat", cgc, SaveNameFunction)).ToListAsync();

        // API key が違うコンテキストは現在の API key で作り直す
        var differentApiKeyContexts = _chatGptContexts.Where(context => context.Context.ApiKey != _apiKeyService.OpenAiApiKey).ToArray();
        foreach (var context in differentApiKeyContexts)
        {
            _chatGptContexts.Remove(context);
            _chatGptContexts.Add(new ChatContext(context.Name, new(_apiKeyService.OpenAiApiKey, context.Context), SaveNameFunction));
        }

        if (!_chatGptContexts.Any())
        {
            AddChatContext(CreateChatContext("new chat"));
        }

        return _chatGptContexts;
    }

    public async Task<ChatContext> GetLastChatGptContextAsync() => lastChatGptContext ??= (await GetChatGptContextsAsync())[0];

    public void SetLastChatGptContext(ChatContext value) => lastChatGptContext = value;

    public bool IsOpenAIApiKeyNotSet => string.IsNullOrEmpty(_apiKeyService.OpenAiApiKey);

    public ContextService(IApiKeyService apiKeyService, ILogStorageService logStorageService)
    {
        HttpClient = new HttpClient();
        _apiKeyService = apiKeyService;
        _logStorageService = logStorageService;
    }

    public ChatContext CreateChatContext(string name) => new(name, new ChatGptContext(_apiKeyService.OpenAiApiKey, HttpClient, name), SaveNameFunction);
    public void AddChatContext(ChatContext chatContext) => _chatGptContexts?.Add(chatContext);

    public void RemoveChatContext(ChatContext chatContext)
    {
        if (_chatGptContexts is null)
        {
            return;
        }

        _chatGptContexts.Remove(chatContext);
        _logStorageService.RemoveContext(chatContext.Context.Id);
    }

    public void SaveChatContext(ChatContext chatContext)
    {
        _logStorageService.SaveContextAsync(chatContext.Context);
    }
}
