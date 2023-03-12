using System.Collections.ObjectModel;
using AIChat.Contracts.Services;
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
    private ChatContext? _currentContext;
    private readonly IContextService _contextService;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveContextCommand))]
    private bool _canRemoveContext;

    public ObservableCollection<ChatContext>? ChatGptContexts
    {
        get;
        private set;
    }

    public ObservableCollection<ChatMessage> Messages
    {
        get;
    }

    public ChatGptViewModel(IContextService contextService)
    {
        _inputText = string.Empty;
        Messages = new();

        _contextService = contextService;

        Task.Run(async () =>
        {
            // チャットコンテキストの設定
            ChatGptContexts = new(await _contextService.GetChatGptContextsAsync());

            if (_contextService.IsOpenAIApiKeyNotSet)
            {
                Messages.Add(new(Guid.NewGuid(),
                    "APP",
                    """
                    OpenAI API key is not set.
                    Please enter it from the settings page.
                    """
                , 0));
            }

            ChatGptContexts.CollectionChanged += async (sender, e) =>
            {
                var contexts = await _contextService.GetChatGptContextsAsync();
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var context in e.NewItems?.Cast<ChatContext>() ?? Enumerable.Empty<ChatContext>())
                        {
                            _contextService.AddChatContext(context);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var context in e.OldItems?.Cast<ChatContext>() ?? Enumerable.Empty<ChatContext>())
                        {
                            _contextService.RemoveChatContext(context);
                        }
                        break;
                }

                CanRemoveContext = ChatGptContexts.Count > 1;
            };

            await InitializeContext();
        });

        PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentContext):
                    if (CurrentContext is null)
                    {
                        break;
                    }

                    Messages.Clear();
                    foreach (var message in CurrentContext.Context.MessageLog.Select(m => new ChatMessage(m)))
                    {
                        Messages.Add(message);
                    }
                    _contextService.SetLastChatGptContext(CurrentContext);
                    break;
            }
        };
    }

    public async Task InitializeContext()
    {
        CurrentContext = await _contextService.GetLastChatGptContextAsync();
    }

    [RelayCommand]
    private void AddContext()
    {
        if (ChatGptContexts is null)
        {
            return;
        }

        var newChat = _contextService.CreateChatContext("new chat");
        ChatGptContexts.Add(newChat);
        CurrentContext = newChat;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveContext))]
    private void RemoveContext()
    {
        if (ChatGptContexts is null || CurrentContext is null)
        {
            return;
        }

        var targetContext = CurrentContext;
        var nextSelection = ChatGptContexts.ElementAtOrDefault(ChatGptContexts.IndexOf(targetContext) + 1) ?? ChatGptContexts[^2];

        CurrentContext = nextSelection;
        ChatGptContexts.Remove(targetContext);
    }

    [RelayCommand]
    private void Clean()
    {
        if (CurrentContext is null)
        {
            return;
        }

        CurrentContext.Context.MessageLog.Clear();
        Messages.Clear();
    }

    [RelayCommand]
    private async Task Tell()
    {
        if (CurrentContext is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(InputText)) { return; }
        var input = InputText;
        var asSystem = AsSystem;

        InputText = string.Empty;
        AsSystem = false;

        var responses = asSystem ? CurrentContext.Context.TellAsSystem(input) : CurrentContext.Context.TellAsUser(input);
        ChatMessage? first = null;
        await foreach (var message in responses)
        {
            var m = new ChatMessage(message);
            Messages.Add(m);
            first ??= m;
        }

        // 帰ってくる最初のメッセージはプロンプト
        // レスポンスが帰ってきた後にトークンが入っているので更新する
        first?.Reflesh();

        _contextService.SaveChatContext(CurrentContext);
    }
}
