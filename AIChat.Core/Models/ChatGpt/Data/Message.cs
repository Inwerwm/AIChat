using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

internal record Message(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content
);
