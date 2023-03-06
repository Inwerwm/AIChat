using AIChat.Core.Models.ChatGpt;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AIChat.Models;
public partial class ChatContextItem : ObservableRecipient
{
    [ObservableProperty]
    private string _name;

    public ChatGptContext Context
    {
        get;
    }

    public ChatContextItem(string name, ChatGptContext context)
    {
        _name = name;
        Context = context;
    }
}
