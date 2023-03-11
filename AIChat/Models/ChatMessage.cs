using AIChat.Core.Models.ChatGpt;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUIEx.Messaging;

namespace AIChat.Models;
public partial class ChatMessage : ObservableRecipient, IEquatable<ChatMessage>
{
    private readonly ChatGptMessage _message;
    
    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string _role;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private int _tokens;

    public ChatMessage(ChatGptMessage message)
    {
        _message = message;

        _id = message.Id;
        _role = message.Role;
        _content = message.Content;
        _tokens = message.Tokens;
    }

    public ChatMessage(Guid id, string role, string content, int tokens)
    {
        _id = id;
        _role = role;
        _content = content;
        _tokens = tokens;

        _message = new(id, role, content, tokens);
    }

    public void Reflesh()
    {
        Id = _message.Id;
        Role = _message.Role;
        Content = _message.Content;
        Tokens = _message.Tokens;
    }

    public bool Equals(ChatMessage? other) => other is not null && _message.Equals(other._message);
    public override bool Equals(object? obj) => obj is ChatMessage message && Equals(message);
    public override int GetHashCode() => _message.GetHashCode();
}
