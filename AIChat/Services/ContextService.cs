using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIChat.Contracts.Services;
using AIChat.Core.Models.ChatGpt;
using AIChat.Models;

namespace AIChat.Services;
internal class ContextService : IContextService
{
    private readonly IApiKeyService _apiKeyService;
    private readonly List<ChatContext> chatGptContexts = new();
    private ChatContext? lastChatGptContext;

    public HttpClient HttpClient
    {
        get;
    }

    public List<ChatContext> ChatGptContexts
    {
        get
        {
            // API key が違うコンテキストは現在の API key で作り直す
            var differentApiKeyContexts = chatGptContexts.Where(context => context.Context.ApiKey != _apiKeyService.OpenAiApiKey).ToArray();
            foreach (var context in differentApiKeyContexts)
            {
                chatGptContexts.Remove(context);
                chatGptContexts.Add(new(context.Name, new(_apiKeyService.OpenAiApiKey, context.Context)));
            }

            if (!chatGptContexts.Any())
            {
                chatGptContexts.Add(CreateChatContext("new chat"));
            }

            return chatGptContexts;
        }
    }

    public ChatContext LastChatGptContext
    {
        get => lastChatGptContext ??= ChatGptContexts.First();
        set => lastChatGptContext = value;
    }

    public bool IsOpenAIApiKeyNotSet => string.IsNullOrEmpty(_apiKeyService.OpenAiApiKey);

    public ContextService(IApiKeyService apiKeyService)
    {
        HttpClient = new HttpClient();
        _apiKeyService = apiKeyService;
    }

    public ChatContext CreateChatContext(string name) => new(name, new(HttpClient, _apiKeyService.OpenAiApiKey));
}
