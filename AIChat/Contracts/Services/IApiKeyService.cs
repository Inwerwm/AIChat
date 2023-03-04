using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIChat.Contracts.Services;
public interface IApiKeyService
{
    string OpenAiApiKey
    {
        get;
        set;
    }
}
