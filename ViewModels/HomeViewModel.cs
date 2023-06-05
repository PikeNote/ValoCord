using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ValCord.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string date = "Today is ";

    private static string getDate()
    {
        return DateTime.Now.Date.ToString("MM/dd/yyyy");
    }
}