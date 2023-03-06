using System.Collections.ObjectModel;
using AIChat.Contracts.Services;
using AIChat.Core.Models.ChatGpt;
using AIChat.Core.Models.ChatGpt.Data;
using AIChat.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIChat.ViewModels;

public partial class ChatGptViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _inputText;
    [ObservableProperty]
    private bool _asSystem;
    private ChatGptContext _chatGptContext;
    private IContextService _contextService;

    public ObservableCollection<ChatContextItem> ChatGptContexts
    {
        get;
    }

    private ChatGptContext ChatGptContext
    {
        get => _chatGptContext;
        set
        {
            _chatGptContext = value;
            Messages.Clear();
            foreach (var message in _chatGptContext.MessageLog)
            {
                Messages.Add(message);
            }
        }
    }

    public ObservableCollection<Message> Messages
    {
        get;
    }

    public ChatGptViewModel(IContextService contextService)
    {
        _inputText = string.Empty;
        Messages = new();

        _contextService = contextService;
        ChatGptContexts = new(_contextService.ChatGptContexts.Select(context => new ChatContextItem("context", context)));
        _chatGptContext = ChatGptContexts.First().Context;

        if (_contextService.IsOpenAIApiKeyNotSet)
        {
            Messages.Add(new(
                "APP",
                """
                OpenAI API key is not set.
                Please enter it from the settings page.
                """
            ));
        }
    }

    public void ChangeContext(ChatContextItem contextItem)
    {
        ChatGptContext = contextItem.Context;
    }

    public void AddContext()
    {
        var newContext = _contextService.CreateChatGptContext();
        _contextService.ChatGptContexts.Add(newContext);
        ChatGptContexts.Add(new($"context {_contextService.ChatGptContexts.Count}", newContext));
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
