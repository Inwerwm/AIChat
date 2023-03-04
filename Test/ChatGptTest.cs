using System.Text.Json;
using AIChat.Core.Models.ChatGpt;
using Xunit.Abstractions;

namespace Test;

public class ChatGptTest
{
    private readonly ITestOutputHelper output;
    public HttpClient Client { get; set; } = new();

    public ChatGptTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async void AccessTest()
    {
        var context = new ChatGptContext(Client, Secret.ApiKey);
        var response = await context.Request("Chat GPT について説明してください。");

        Assert.NotNull(response);

        output.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
