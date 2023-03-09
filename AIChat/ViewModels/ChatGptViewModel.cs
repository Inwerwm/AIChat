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
    [ObservableProperty]
    private ChatContext _currentContext;
    private readonly IContextService _contextService;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveContextCommand))]
    private bool _canRemoveContext;

    public ObservableCollection<ChatContext> ChatGptContexts
    {
        get;
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

        // チャットコンテキストの設定
        ChatGptContexts = new(_contextService.ChatGptContexts);
        ChatGptContexts.CollectionChanged += (sender, e) =>
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    _contextService.ChatGptContexts.AddRange(e.NewItems?.Cast<ChatContext>() ?? Enumerable.Empty<ChatContext>());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    _contextService.ChatGptContexts.RemoveAll(item => e.OldItems?.Contains(item) ?? false);
                    break;
            }

            CanRemoveContext = ChatGptContexts.Count > 1;
        };
        // コンストラクタの nullable エラー避け
        // ページ描画時に InitializeContextが呼ばれるので実質無意味
        _currentContext = _contextService.LastChatGptContext;

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

        PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentContext):
                    Messages.Clear();
                    foreach (var message in CurrentContext.Context.MessageLog)
                    {
                        Messages.Add(message);
                    }
                    _contextService.LastChatGptContext = CurrentContext;
                    break;
            }
        };
    }

    public void InitializeContext()
    {
        CurrentContext = _contextService.LastChatGptContext;
    }

    [RelayCommand]
    private void AddContext()
    {
        var newChat = _contextService.CreateChatContext("new chat");
        ChatGptContexts.Add(newChat);
        CurrentContext = newChat;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveContext))]
    private void RemoveContext()
    {
        var targetContext = CurrentContext;
        var nextSelection = ChatGptContexts.ElementAtOrDefault(ChatGptContexts.IndexOf(targetContext) + 1) ?? ChatGptContexts[^2];

        CurrentContext = nextSelection;
        ChatGptContexts.Remove(targetContext);
    }

    [RelayCommand]
    private void Clean()
    {
        CurrentContext.Context.MessageLog.Clear();
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

        var responses = asSystem ? CurrentContext.Context.TellAsSystem(input) : CurrentContext.Context.TellAsUser(input);
        await foreach (var message in responses)
        {
            Messages.Add(message);
        }
    }
}
