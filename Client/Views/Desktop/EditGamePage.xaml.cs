using Client.Models;
using Client.Services;
using GameLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Views.Desktop;

public partial class EditGamePage : ContentPage
{
    private Products editModel;
    private readonly IProductService productService;

    public EditGamePage(ProductTem game)
    {
        InitializeComponent();

        // Create an editable Products instance from ProductTem
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

        BindingContext = editModel;

        // Resolve IProductService from available service providers. Prefer App.Services, fallback to MauiContext.Services.
        IServiceProvider? provider = (Application.Current as App)?.Services;
        if (provider == null)
        {
            System.Diagnostics.Debug.WriteLine("App.Services was null, attempting MauiContext.Services fallback");
            provider = Application.Current?.Handler?.MauiContext?.Services;
        }

        if (provider == null)
            throw new InvalidOperationException("IServiceProvider not available");

        productService = provider.GetService<IProductService>() ?? throw new InvalidOperationException("IProductService not available");

        // Load genres for Picker
        _ = LoadGenres();
    }

    private async Task LoadGenres()
    {
        try
        {
            var gl = await productService.GetGenresAsync();
            // Bind the picker directly to client genre models
            GenrePicker.ItemsSource = gl;

            // Pre-select the current genre
            var idx = gl.FindIndex(g => g.Id == editModel.GenreId);
            if (idx >= 0)
                GenrePicker.SelectedIndex = idx;

            // Set the date picker
            try
            {
                ReleaseDatePicker.Date = editModel.ReleasedDate.ToDateTime(new TimeOnly(0, 0));
            }
            catch
            {
                // ignore
            }

            // Preview image
            if (!string.IsNullOrWhiteSpace(editModel.Image))
            {
                try
                {
                    var img = Convert.FromBase64String(editModel.Image);
                    PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(img));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Preview image failed: {ex}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadGenres failed: {ex}");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            // Ensure fields are valid
            if (string.IsNullOrWhiteSpace(editModel.Name) || string.IsNullOrWhiteSpace(editModel.Description))
            {
                await DisplayAlertAsync("Validation", "Name and Description are required.", "OK");
                return;
            }

            // Update values from UI controls (picker + date)
            try
            {
                if (GenrePicker.SelectedItem is Client.Models.Genre selGenre)
                {
                    editModel.GenreId = selGenre.Id;
                }
            }
            catch { }

            try
            {
                // ReleaseDatePicker.Date may be nullable on some platforms; guard it
                var dt = ReleaseDatePicker.Date;
                editModel.ReleasedDate = DateOnly.FromDateTime(dt ?? DateTime.Now);
            }
            catch { }

            var result = await productService.UpdateProductAsync(editModel);
            if (result.Success)
            {
                await DisplayAlertAsync("Success", result.Message ?? "Updated", "OK");
                // Optionally refresh data by sending a message or relying on parent to refresh
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync("Error", result.Message ?? "Failed to update", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnSaveClicked exception: {ex}");
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.Navigation.PopAsync();
    }
}