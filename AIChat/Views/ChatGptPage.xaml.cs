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

    private void ContextNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        ViewModel.ChangeContext((ChatContextItem)args.SelectedItem);
    }

    private void NavigationViewItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        ViewModel.AddContext();
    }
}
