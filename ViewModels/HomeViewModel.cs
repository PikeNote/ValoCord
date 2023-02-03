using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValCord.ViewModels;
public class HomeViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public string Date
    {
        get
        {
            return "Today is " + DateTime.Now.Date.ToString("MM/dd/yyyy");
        }
    }
}