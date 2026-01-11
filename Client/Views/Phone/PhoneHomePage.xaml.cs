using Client.ViewModels;
using Client.Models;
using System.Linq;

namespace Client.Views.Phone;

public partial class PhoneHomePage : ContentPage
{
	private PhoneHomePageViewModel? ViewModel => BindingContext as PhoneHomePageViewModel;

	public PhoneHomePage(PhoneHomePageViewModel phoneHomePageViewModel)
	{
		InitializeComponent();
		BindingContext = phoneHomePageViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (ViewModel != null)
		{
			ViewModel.RefreshGamesCommand.Execute(null);
		}
	}

	private async void OnGameSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var selected = e.CurrentSelection.FirstOrDefault() as ProductTem;
		if (selected == null)
			return;

		// Clear selection immediately so it doesn't stay highlighted
		if (sender is CollectionView cv)
		{
			cv.SelectedItem = null;
		}

		try
		{
			var page = new PhoneGameDetailPage(selected);
			await Shell.Current.Navigation.PushAsync(page);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Navigation to PhoneGameDetailPage failed: {ex}");
		}
	}
}