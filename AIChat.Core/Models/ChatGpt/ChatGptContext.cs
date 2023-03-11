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

    private static string ApiUrl
    {
        get;
    } = "https://api.openai.com/v1/chat/completions";

    public string ApiKey
    {
        get;
    }

    public List<ChatGptMessage> MessageLog
    {
        get;
    }

    private readonly int _tokenLimit = 4096;
    private readonly int _tokenBuffer = 512;

    public int TotalTokens
    {
        get;
        private set;
    }

    public ChatGptContext(HttpClient client, string apiKey)
    {
        Client = client;
        ApiKey = apiKey;
        MessageLog = new();
    }

    public ChatGptContext(HttpClient client, string apiKey, List<ChatGptMessage> messageLog) : this(client, apiKey)
    {
        MessageLog = messageLog;
    }

    public ChatGptContext(string apiKey, ChatGptContext other) : this(other.Client, apiKey, other.MessageLog) { }

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

    private async IAsyncEnumerable<ChatGptMessage> Tell(Role role, string content, bool requireSubmit = true)
    {
        // リクエストではこれまでの全会話を送るのではじめに入力メッセージをログに追加する
        var promptMessage = new ChatGptMessage(Guid.NewGuid(), role.GetString(), content, 0);
        MessageLog.Add(promptMessage);

        // まず入力メッセージを返す
        yield return promptMessage;
        if (!requireSubmit) { yield break; }

        // 帰ってくるプロンプトのコスト数は一緒に送ったログの値も含むので
        // プロンプト単体のコスト算出のためログ全体のトークン数を持っておく
        var logsToSend = MessageLog.TakeByLimitDesc(_tokenLimit - _tokenBuffer, m => m.Tokens).ToList();
        var contextTokens = logsToSend.Sum(l => l.Tokens);

        // リクエストを送信
        var request = new RequestBody() { Model = "gpt-3.5-turbo", Messages = logsToSend.Select(m => new Message(m.Role, m.Content)).ToList() };
        using var httpResponse = await Request(request);
        if (httpResponse is null || !httpResponse.IsSuccessStatusCode)
        {
            // 通信に失敗したらチャットログには入れずにエラーメッセージを返す
            yield return CreateErrorMessage(httpResponse);
            yield break;
        }
        var response = await httpResponse.Content.ReadFromJsonAsync<Response>();
        if (response is null)
        {
            // Jsonからのデシリアライズに失敗したとき
            yield return new(Guid.NewGuid(), "ERROR", """
                Response Error.
                Failed to deserialize response data from Json.
                """
            , 0);
            yield break;
        }

        // レスポンスからプロンプトのトークン数を得る
        promptMessage.Tokens = response.Usage.PromptTokens - contextTokens;
        TotalTokens = response.Usage.TotalTokens;
        // レスポンスをチャットログに追加するとともに戻り値として返す
        var responseMessages = response.Choices.Select(c => new ChatGptMessage(Guid.NewGuid(), c.Message.Role, c.Message.Content, (int)Math.Floor(response.Usage.CompletionTokens / (double)response.Choices.Length)));
        MessageLog.AddRange(responseMessages);

        foreach (var responseMessage in responseMessages)
        {
            yield return responseMessage;
        }
    }

    private static ChatGptMessage CreateErrorMessage(HttpResponseMessage? httpResponse) => new(Guid.NewGuid(), "ERROR", httpResponse is null ?
        """
            API Access error.
            The cause of the error is unknown.
            """ :
        $"""
            API Access error.
            Status code is {httpResponse.StatusCode}
            """
        , 0);

    /// <summary>
    /// 動作設定をするためのメッセージを送る
    /// </summary>
    public IAsyncEnumerable<ChatGptMessage> TellAsSystem(string message) => Tell(Role.System, message);
    /// <summary>
    /// チャットを送る
    /// </summary>
    public IAsyncEnumerable<ChatGptMessage> TellAsUser(string message) => Tell(Role.User, message);
    /// <summary>
    /// AIの回答を装った会話を追加する
    /// </summary>
    public IAsyncEnumerable<ChatGptMessage> TellAsAssistant(string message) => Tell(Role.Assistant, message, false);
}
