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

    public string ApiKey
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

    private async Task<HttpResponseMessage?> Request(RequestBody requestBody)
    {
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        Client.DefaultRequestHeaders.Add("X-Slack-No-Retry", "1");

        var content = JsonContent.Create(requestBody, mediaType: new MediaTypeHeaderValue("application/json"), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        return await Client.PostAsync(ApiUrl, content);
    }

    private async IAsyncEnumerable<Message> Tell(Role role, string content, bool requireSubmit = true)
    {
        // リクエストではこれまでの全会話を送るのではじめに入力メッセージをログに追加する
        var message = new Message(role.GetString(), content);
        MessageLog.Add(message);

        yield return message;
        if(!requireSubmit) { yield break; }

        var request = new RequestBody() { Model = "gpt-3.5-turbo", Messages = MessageLog };

        using var httpResponse = await Request(request);
        if (httpResponse is null || !httpResponse.IsSuccessStatusCode)
        {
            yield return new("ERROR", httpResponse is null ?
                """
                    API Access error.
                    The cause of the error is unknown.
                    """ :
                $"""
                    API Access error.
                    Status code is {httpResponse.StatusCode}
                    """
            );
            yield break;
        }
        var response = await httpResponse.Content.ReadFromJsonAsync<Response>();
        if (response is null) { yield break; }

        var responseMessages = response.Choices.Select(c => c.Message);
        MessageLog.AddRange(responseMessages);

        foreach (var responseMessage in responseMessages)
        {
            yield return responseMessage;
        }
    }

    /// <summary>
    /// 動作設定をするためのメッセージを送る
    /// </summary>
    public IAsyncEnumerable<Message> TellAsSystem(string message) => Tell(Role.System, message);
    /// <summary>
    /// チャットを送る
    /// </summary>
    public IAsyncEnumerable<Message> TellAsUser(string message) => Tell(Role.User, message);
    /// <summary>
    /// AIの回答を装った会話を追加する
    /// </summary>
    public IAsyncEnumerable<Message> TellAsAssistant(string message) => Tell(Role.Assistant, message, false);
}
