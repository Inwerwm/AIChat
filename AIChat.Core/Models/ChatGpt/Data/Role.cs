using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIChat.Core.Models.ChatGpt.Data;
internal enum Role
{
    System,
    User,
    Assistant
}

internal static class RoleConverter
{
    public static string GetString(this Role role) => role switch
    {
        Role.System => "system",
        Role.User => "user",
        Role.Assistant => "assistant",
        _ => throw new NotImplementedException(),
    };

    public static Role FromString(string role) => role switch
    {
        "system" => Role.System,
        "user" => Role.User,
        "assistant" => Role.Assistant,
        _ => throw new InvalidOperationException(),
    };
}
