using System.Reflection;
using System.Windows.Input;

using AIChat.Contracts.Services;
using AIChat.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace AIChat.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IApiKeyService _apiKeyService;
    private ElementTheme _elementTheme;
    private string _versionDescription;

    [ObservableProperty]
    private string _openAiApiKey;

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(ILocalSettingsService localSettingsService, IThemeSelectorService themeSelectorService, IApiKeyService apiKeyService)
    {
        _localSettingsService = localSettingsService;
        _themeSelectorService = themeSelectorService;
        _apiKeyService = apiKeyService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });

        _openAiApiKey = _apiKeyService.OpenAiApiKey;
        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(OpenAiApiKey))
            {
                _apiKeyService.OpenAiApiKey = OpenAiApiKey;
            }
        };
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
