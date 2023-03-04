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

    public List<Message> Messages
    {
        get;
    }

    public ChatGptContext(HttpClient client, string apiKey)
    {
        Client = client;
        ApiKey = apiKey;
        Messages = new();
    }

    public async Task<Response?> Request(string message)
    {
        Messages.Add(new Message("user", message));
        var requestBody = new RequestBody() { Model = "gpt-3.5-turbo", Messages = Messages };

        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        Client.DefaultRequestHeaders.Add("X-Slack-No-Retry", "1");

        var content = JsonContent.Create(requestBody, mediaType: new MediaTypeHeaderValue("application/json"), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        using var response = await Client.PostAsync(ApiUrl, content);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Response>() : null;
    }
}
