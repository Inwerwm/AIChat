using AIChat.Core.Models.ChatGpt;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AIChat.Models;
public partial class ChatContext : ObservableRecipient
{
    [ObservableProperty]
    private string _name;

    public ChatGptContext Context
    {
        get;
    }

    public ChatContext(string name, ChatGptContext context)
    {
        _name = name;
        Context = context;
    }
}
