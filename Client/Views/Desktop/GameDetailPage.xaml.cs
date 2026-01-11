using Client.Models;
using Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using GameLibrary.Responses;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.Views.Desktop;

public partial class GameDetailPage : ContentPage
{
    private readonly ProductTem gameModel;

    public GameDetailPage(ProductTem game)
    {
        InitializeComponent();
        BindingContext = game;
        gameModel = game;
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("OnEditClicked invoked");
        if (Shell.Current?.Navigation == null)
        {
            System.Diagnostics.Debug.WriteLine("Navigation is not available on Shell.Current");
            await DisplayAlertAsync("Navigation Error", "Navigation is not available. Try opening the page from the main shell.", "OK");
            return;
        }

        try
        {
            var page = new EditGamePage(gameModel);
            await Shell.Current.Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnEditClicked navigation failed: {ex}");
            await DisplayAlertAsync("Navigation Failed", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        try
        {
            var confirm = await DisplayAlertAsync("Delete", "Are you sure you want to delete this game?", "Yes", "No");
            if (!confirm) return;

            // Resolve service
            IServiceProvider? provider = (Application.Current as App)?.Services;
            if (provider == null)
            {
                provider = Application.Current?.Handler?.MauiContext?.Services;
            }

            var productService = provider?.GetService<IProductService>();
            if (productService == null)
            {
                await DisplayAlertAsync("Error", "Delete service not available.", "OK");
                return;
            }

            var resp = await productService.DeleteProductAsync(gameModel.Id);
            if (resp.Success)
            {
                // Notify list to refresh via WeakReferenceMessenger
                try
                {
                    CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Client.Messages.GameDeletedMessage(gameModel.Id), "GameDeleted");
                }
                catch { }

                await DisplayAlertAsync("Deleted", resp.Message ?? "Deleted", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync("Error", resp.Message ?? "Failed to delete", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnDeleteClicked failed: {ex}");
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}