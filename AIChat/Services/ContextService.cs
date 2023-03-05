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
    public HttpClient HttpClient
    {
        get;
    }

    private ChatGptContext? ChatGptContext
    {
        get;
        set;
    }

    public ContextService()
    {
        HttpClient = new HttpClient();
    }

    public ChatGptContext GetChatGptContext(string apiKey)
    {
        if(ChatGptContext is null || ChatGptContext.ApiKey != apiKey)
        {
            ChatGptContext = new ChatGptContext(HttpClient, apiKey);
        }

        return ChatGptContext;
    }
}
