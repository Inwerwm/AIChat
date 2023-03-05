using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AIChat.Core.Models.ChatGpt.Data;

namespace AIChat.Core.Models.ChatGpt;
public class ChatGptContext
{
    private HttpClient Client
    {
        get;
    }

    private string ApiUrl
    {
        get;
    } = "https://api.openai.com/v1/chat/completions";

    private string ApiKey
    {
        get;
    }

    public List<Message> MessageLog
    {
        get;
    }

    public ChatGptContext(HttpClient client, string apiKey)
    {
        Client = client;
        ApiKey = apiKey;
        MessageLog = new();
    }

    private async Task<Response?> Request(RequestBody requestBody)
    {
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        Client.DefaultRequestHeaders.Add("X-Slack-No-Retry", "1");

        var content = JsonContent.Create(requestBody, mediaType: new MediaTypeHeaderValue("application/json"), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        using var httpResponse = await Client.PostAsync(ApiUrl, content);
        return httpResponse.IsSuccessStatusCode ? await httpResponse.Content.ReadFromJsonAsync<Response>() : null;
    }

    private async Task<IEnumerable<Message>> Tell(Role role, string message)
    {
        // リクエストではこれまでの全会話を送るのではじめに入力メッセージをログに追加する
        MessageLog.Add(new Message(role.GetString(), message));
        var request = new RequestBody() { Model = "gpt-3.5-turbo", Messages = MessageLog };

        var response = await Request(request);
        if (response is null) { return Enumerable.Empty<Message>(); }

        var responseMessages = response.Choices.Select(c => c.Message);
        MessageLog.AddRange(responseMessages);
        return responseMessages;
    }

    /// <summary>
    /// モデルの動作設定をするためのメッセージを送る
    /// </summary>
    public async Task<IEnumerable<Message>> TellAsSystem(string message) => await Tell(Role.System, message);
    public async Task<IEnumerable<Message>> TellAsUser(string message) => await Tell(Role.User, message);
    public IEnumerable<Message> TellAsAssistant(string message)
    {
        var m = new Message(Role.Assistant.GetString(), message);
        MessageLog.Add(m);
        return new[] { m };
    }
}
