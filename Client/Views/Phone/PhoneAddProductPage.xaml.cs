using Client.ViewModels;

namespace Client.Views.Phone;

public partial class PhoneAddProductPage : ContentPage
{
    private readonly AddProductPageViewModel vm;

    public PhoneAddProductPage(AddProductPageViewModel vm)
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
