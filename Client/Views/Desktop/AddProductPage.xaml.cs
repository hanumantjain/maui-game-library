using Client.ViewModels;

namespace Client.Views.Desktop;

public partial class AddProductPage : ContentPage
{
    private readonly AddProductPageViewModel vm;

    public AddProductPage(AddProductPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = this.vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializeAsync();
    }
}
