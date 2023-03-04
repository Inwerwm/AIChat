using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

public record Message(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content
);
