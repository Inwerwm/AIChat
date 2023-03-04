using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

public class Response
{
    [JsonPropertyName("id")]
    public required string Id
    {
        get; init;
    }

    [JsonPropertyName("object")]
    public required string Object
    {
        get; init;
    }

    [JsonPropertyName("created")]
    public required int Created
    {
        get; init;
    }

    [JsonPropertyName("choices")]
    public required Choice[] Choices
    {
        get; init;
    }

    [JsonPropertyName("usage")]
    public required Usage Usage
    {
        get; init;
    }
}
