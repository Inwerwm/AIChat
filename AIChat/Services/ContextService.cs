using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIChat.Contracts.Services;
using AIChat.Core.Models.ChatGpt;

namespace AIChat.Services;
internal class ContextService : IContextService
{
    private readonly IApiKeyService _apiKeyService;
    private readonly List<ChatGptContext> chatGptContexts = new();

    public HttpClient HttpClient
    {
        get;
    }

    public List<ChatGptContext> ChatGptContexts
    {
        get
        {
            // API key が違うコンテキストは現在の API key で作り直す
            var differentApiKeyContexts = chatGptContexts.Where(context => context.ApiKey != _apiKeyService.OpenAiApiKey).ToArray();
            foreach (var context in differentApiKeyContexts)
            {
                chatGptContexts.Remove(context);
                chatGptContexts.Add(new(_apiKeyService.OpenAiApiKey, context));
            }

            if (!chatGptContexts.Any())
            {
                chatGptContexts.Add(CreateChatGptContext());
            }

            return chatGptContexts;
        }
    }

    public bool IsOpenAIApiKeyNotSet => string.IsNullOrEmpty(_apiKeyService.OpenAiApiKey);

    public ContextService(IApiKeyService apiKeyService)
    {
        HttpClient = new HttpClient();
        _apiKeyService = apiKeyService;
    }

    public ChatGptContext CreateChatGptContext() => new(HttpClient, _apiKeyService.OpenAiApiKey);
}
