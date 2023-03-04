using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

public class Choice
{
    [JsonPropertyName("index")]
    public required int Index
    {
        get; init;
    }

    [JsonPropertyName("message")]
    public required Message Message
    {
        get; init;
    }

    [JsonPropertyName("finish_reason")]
    public required string FinishReason
    {
        get; init;
    }
}
