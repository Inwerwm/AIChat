using AIChat.Contracts.Services;
using AIChat.Core.Contracts.Services;
using AIChat.Core.Models.ChatGpt;
using AIChat.Models;
using Microsoft.Extensions.Options;

namespace AIChat.Services;
internal class LogStorageService : ILogStorageService
{
    private const string _contextsSettingKey = "contexts";
    private const string _messagesSettingsKey = "messages";
    private const string _nameSettingKey = "name";
    private const string _totalTokensSettingsKey = "total_tokens";
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IFileService _fileService;
    private readonly IApiKeyService _apiKeyService;

    private Dictionary<Guid, ILocalSettingsService> ContextLocalSettingServices { get; } = new();

    public LogStorageService(ILocalSettingsService localSettingsService, IFileService fileService, IApiKeyService apiKeyService)
    {
        _localSettingsService = localSettingsService;
        _fileService = fileService;
        _apiKeyService = apiKeyService;
    }

    private ILocalSettingsService CreateContextService(Guid contextId) => new LocalSettingsService(_fileService, new LocalSettingsOption()
    {
        Value = new()
        {
            LocalSettingsFile = $"{contextId}.json"
        }
    });

    public async Task SaveContextAsync(ChatGptContext context)
    {
        if (!ContextLocalSettingServices.TryGetValue(context.Id, out var contextService))
        {
            // コンテキストが未保存の場合、新しく設定サービスを作成する。
            contextService = CreateContextService(context.Id);
            ContextLocalSettingServices.Add(context.Id, contextService);

            // ロードするファイルの名前の特定のために、保存コンテキスト一覧を保存する。
            await _localSettingsService.SaveSettingAsync(_contextsSettingKey, ContextLocalSettingServices.Keys.ToArray());
        }

        await contextService.SaveSettingAsync(_nameSettingKey, context.Name);
        await contextService.SaveSettingAsync(_totalTokensSettingsKey, context.TotalTokens);
        await contextService.SaveSettingAsync(_messagesSettingsKey, context.MessageLog);
    }

    public async IAsyncEnumerable<ChatGptContext> LoadContextsAsync(HttpClient httpClient)
    {
        var savedContextIds = await _localSettingsService.ReadSettingAsync<Guid[]>(_contextsSettingKey);
        if (savedContextIds is null)
        {
            yield break;
        }

        ContextLocalSettingServices.Clear();
        foreach (var id in savedContextIds)
        {
            var contextService = CreateContextService(id);
            ContextLocalSettingServices.Add(id, contextService);

            yield return new ChatGptContext(
                _apiKeyService.OpenAiApiKey,
                httpClient,
                await contextService.ReadSettingAsync<string?>(_nameSettingKey) ?? "",
                await contextService.ReadSettingAsync<List<ChatGptMessage>>(_messagesSettingsKey) ?? new(),
                await contextService.ReadSettingAsync<int>(_totalTokensSettingsKey),
                id);
        }
    }

    public void RemoveContext(Guid contextId)
    {
        if(!ContextLocalSettingServices.TryGetValue(contextId, out var context))
        {
            return;
        }

        context.RemoveSetting();
        ContextLocalSettingServices.Remove(contextId);
    }
}

file class LocalSettingsOption : IOptions<LocalSettingsOptions>
{
    public required LocalSettingsOptions Value
    {
        get;
        set;
    }
}