using AIChat.Contracts.Services;

namespace AIChat.Services;
internal class ApiKeyService : IApiKeyService
{
    private readonly ILocalSettingsService _localSettingsService;
    private const string OpenAiApiKeySettingsKey = "OpenAiApiKey";
    private string _openAiApiKey;

    public string OpenAiApiKey
    {
        get => _openAiApiKey;
        set
        {
            _openAiApiKey = value;
            Task.Run(async () => await _localSettingsService.SaveSettingAsync(OpenAiApiKeySettingsKey, _openAiApiKey));
        }
    }

    public ApiKeyService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        _openAiApiKey = string.Empty;
        Task.Run(async () =>
        {
            _openAiApiKey = await _localSettingsService.ReadSettingAsync<string>(OpenAiApiKeySettingsKey) ?? string.Empty;
        });
    }
}
