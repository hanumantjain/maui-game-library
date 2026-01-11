using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;
}
