using AIChat.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AIChat.Views;

public sealed partial class ChatGptPage : Page
{
    public ChatGptViewModel ViewModel
    {
        get;
    }

    public ChatGptPage()
    {
        ViewModel = App.GetService<ChatGptViewModel>();
        InitializeComponent();
    }
}
