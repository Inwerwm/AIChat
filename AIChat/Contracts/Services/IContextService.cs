using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIChat.Core.Models.ChatGpt;

namespace AIChat.Contracts.Services;
public interface IContextService
{
    HttpClient HttpClient
    {
    get;
    }

    ChatGptContext GetChatGptContext(string apiKey);
}
