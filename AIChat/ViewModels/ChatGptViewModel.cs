using System.Collections.ObjectModel;
using AIChat.Core.Models.ChatGpt.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIChat.ViewModels;

public partial class ChatGptViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _inputText;

    public ObservableCollection<Choice> Choices
    {
        get;
    }

    public ChatGptViewModel()
    {
        _inputText = string.Empty;
        Choices = new();
    }

    [RelayCommand]
    private void AddChat()
    {
        if(string.IsNullOrEmpty(InputText)) { return; }

        Choices.Add(new(0, new(
            "User",
            InputText
            ), "test"));
        InputText = string.Empty;
    }
}
