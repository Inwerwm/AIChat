using AIChat.Core.Models.ChatGpt;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace AIChat.Models;
public partial class ChatContext : ObservableRecipient
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private bool _enableRenameMode;

    public ChatGptContext Context
    {
        get;
    }

    public ChatContext(string name, ChatGptContext context)
    {
        _name = name;
        Context = context;
    }

    [RelayCommand]
    private void ToggleRenameMode()
    {
        EnableRenameMode = !EnableRenameMode;
    }

    public void TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ToggleRenameMode();
    }
}
