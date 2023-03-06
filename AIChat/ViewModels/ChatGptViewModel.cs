using System.Collections.ObjectModel;
using AIChat.Contracts.Services;
using AIChat.Core.Models.ChatGpt;
using AIChat.Core.Models.ChatGpt.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIChat.ViewModels;

public partial class ChatGptViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _inputText;
    [ObservableProperty]
    private bool _asSystem;

    private readonly IApiKeyService _apiKeyService;

    private ChatGptContext ChatGptContext
    {
        get;
        set;
    }

    public ObservableCollection<Message> Messages
    {
        get;
    }

    public ChatGptViewModel(IApiKeyService apiKeyService, IContextService contextService)
    {
        _inputText = string.Empty;
        Messages = new();
        _apiKeyService = apiKeyService;

        if (string.IsNullOrEmpty(_apiKeyService.OpenAiApiKey))
        {
            Messages.Add(new(
                "APP",
                """
                OpenAI API key is not set.
                Please enter it from the settings page.
                """
            ));
        }

        ChatGptContext = contextService.GetChatGptContext(_apiKeyService.OpenAiApiKey);
        foreach (var message in ChatGptContext.MessageLog)
        {
            Messages.Add(message);
        }
    }

    [RelayCommand]
    private void Clean()
    {
        ChatGptContext.MessageLog.Clear();
        Messages.Clear();
    }

    [RelayCommand]
    private async Task Tell()
    {
        if (string.IsNullOrEmpty(InputText)) { return; }
        var input = InputText;
        var asSystem = AsSystem;

        InputText = string.Empty;
        AsSystem = false;

        var responses = asSystem ? ChatGptContext.TellAsSystem(input) : ChatGptContext.TellAsUser(input);
        await foreach (var message in responses)
        {
            Messages.Add(message);
        }
    }
}
