using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

internal record Choice(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("message")] Message Message,
    [property: JsonPropertyName("finish_reason")] string FinishReason
);
