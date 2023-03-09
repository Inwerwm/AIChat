using AIChat.Models;
using AIChat.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.InitializeContext();
    }
}
