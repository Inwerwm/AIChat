#nullable disable

using System.Text.Json;
using AIChat.Contracts.Services;
using AIChat.Core.Contracts.Services;
using AIChat.Core.Helpers;
using AIChat.Models;

using Microsoft.Extensions.Options;

namespace AIChat.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "AIChat/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public async Task<T> ReadSettingAsync<T>(string key)
    {
        await InitializeAsync();

        if (_settings != null && _settings.TryGetValue(key, out var obj))
        {
            return obj is T value ? value : default;
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        await InitializeAsync();

        _settings[key] = value;

        await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
    }

    public void RemoveSetting()
    {
        _fileService.Delete(_applicationDataFolder, _localsettingsFile);
    }
}
