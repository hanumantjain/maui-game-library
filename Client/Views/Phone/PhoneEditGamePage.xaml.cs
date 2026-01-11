using Client.Models;
using Client.Services;
using Microsoft.Maui.Controls;

namespace Client.Views.Phone;

public partial class PhoneEditGamePage : ContentPage
{
    private readonly ProductTem selectedGame;
    private Products editModel;
    private IProductService? productService;

    public PhoneEditGamePage(ProductTem game)
    {
        selectedGame = game;
        editModel = new Products
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            GenreId = game.GenreId,
            Price = game.Price,
            ReleasedDate = game.ReleasedDate,
            Image = game.ImageBase64 ?? string.Empty
        };
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadGenres();
    }

    private async Task LoadGenres()
    {
        try
        {
            // Resolve service
            IServiceProvider? provider = (Application.Current as App)?.Services;
            if (provider == null)
            {
                provider = Application.Current?.Handler?.MauiContext?.Services;
            }

            productService = provider?.GetService<IProductService>();
            if (productService == null)
            {
                await DisplayAlertAsync("Error", "Service not available.", "OK");
                return;
            }

            var genres = await productService.GetGenresAsync();
            if (genres != null)
            {
                genreComboBox.ItemsSource = genres;
                var currentGenre = genres.FirstOrDefault(g => g.Id == selectedGame.GenreId);
                if (currentGenre != null)
                {
                    genreComboBox.SelectedItem = currentGenre;
                }
            }

            // Pre-populate fields
            nameEntry.Text = selectedGame.Name;
            descriptionEditor.Text = selectedGame.Description;
            priceEntry.Text = selectedGame.Price.ToString();
            datePickerControl.Date = selectedGame.ReleasedDate.ToDateTime(new TimeOnly(0, 0));
            imagePreview.Source = selectedGame.Image;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadGenres error: {ex}");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate and capture values
            if (string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                await DisplayAlertAsync("Validation Error", "Game name is required.", "OK");
                return;
            }

            editModel.Name = nameEntry.Text;
            editModel.Description = descriptionEditor.Text;
            
            if (!double.TryParse(priceEntry.Text, out var price))
            {
                await DisplayAlertAsync("Validation Error", "Invalid price.", "OK");
                return;
            }
            editModel.Price = price;

            if (genreComboBox.SelectedItem is Genre selectedGenre)
            {
                editModel.GenreId = selectedGenre.Id;
            }

            var selectedDate = datePickerControl.Date;
            editModel.ReleasedDate = DateOnly.FromDateTime(selectedDate ?? DateTime.Now);

            // Call service
            if (productService == null)
            {
                IServiceProvider? provider = (Application.Current as App)?.Services;
                if (provider == null)
                {
                    provider = Application.Current?.Handler?.MauiContext?.Services;
                }
                productService = provider?.GetService<IProductService>();
            }

            if (productService == null)
            {
                await DisplayAlertAsync("Error", "Service not available.", "OK");
                return;
            }

            var result = await productService.UpdateProductAsync(editModel);
            if (result.Success)
            {
                await DisplayAlertAsync("Success", result.Message ?? "Game updated", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync("Error", result.Message ?? "Failed to update", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnSaveClicked error: {ex}");
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
}
