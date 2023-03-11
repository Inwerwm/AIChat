namespace AIChat.Core.Models.ChatGpt;
public record ChatGptMessage
{
    public Guid Id
    {
        get;
        init;
    }
    public string Role
    {
        get;
        init;
    }
    public string Content
    {
        get;
        init;
    }
    public int Tokens
    {
        get;
        internal set;
    }

    public ChatGptMessage(Guid id, string role, string content, int tokens)
    {
        Id = id;
        Role = role;
        Content = content;
        Tokens = tokens;
    }
}
