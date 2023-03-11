using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

internal record Usage(
    [property: JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: JsonPropertyName("completion_tokens")] int CompletionTokens,
    [property: JsonPropertyName("total_tokens")] int TotalTokens
);
