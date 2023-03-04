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
        var context = new ChatGptContext(Client, "");
        var response = await context.Request("Chat GPT ‚É‚Â‚¢‚Äà–¾‚µ‚Ä‚­‚¾‚³‚¢B");

        Assert.NotNull(response);

        foreach (var choice in response.Choices)
        {
            output.WriteLine($"{choice.Message.Role}: {choice.Message.Content}");
        }
    }
}
