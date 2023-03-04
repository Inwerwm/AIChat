using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

public class Message
{
    [JsonPropertyName("role")]
    public required string Role
    {
        get; init;
    }

    [JsonPropertyName("content")]
    public required string Content
    {
        get; init;
    }
}
