using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

public record Response(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("object")] string Object,
    [property: JsonPropertyName("created")] int Created,
    [property: JsonPropertyName("choices")] Choice[] Choices,
    [property: JsonPropertyName("usage")] Usage Usage
);
