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
        var context = new ChatGptContext(Secret.ApiKey, Client, "");
        var responses = context.TellAsUser("Chat GPT ‚É‚Â‚¢‚Äà–¾‚µ‚Ä‚­‚¾‚³‚¢B");

        Assert.NotNull(responses);

        await foreach (var response in responses)
        {
            output.WriteLine(response.ToString());
        }
    }
}
