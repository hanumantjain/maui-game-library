using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Client.ViewModels;

public partial class AddProductPageViewModel : BaseViewModel
{
    public ObservableRangeCollection<Genre> Genres { get; } = new();

    [ObservableProperty]
    private Products addProductModel;

    [ObservableProperty]
    private ImageSource? imageSourceFile;

    [ObservableProperty]
    private Genre selectedGenre;

    [ObservableProperty]
    private DateTime releaseDate = DateTime.Now;

    private readonly IProductService productService;

    public AddProductPageViewModel(IProductService productService)
    {
        Title = "Add Game";
        AddProductModel = new Products();
        this.productService = productService;
    }

    private void AppendLog(string message)
    {
        try
        {
            var logPath = Path.Combine(FileSystem.CacheDirectory, "addimage.log");
            File.AppendAllText(logPath, $"{DateTime.UtcNow:o} {message}{Environment.NewLine}");
        }
        catch
        {
            // Best-effort logging
        }

        System.Diagnostics.Debug.WriteLine(message);
    }

    [RelayCommand]
    public async Task AddImage()
    {
        try
        {
            AppendLog("AddImage invoked");

            var pickerTask = FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });

            var completedTask = await Task.WhenAny(
                pickerTask,
                Task.Delay(TimeSpan.FromSeconds(10))
            );

            if (completedTask != pickerTask)
            {
                AppendLog("FilePicker timed out");

                await Shell.Current.DisplayAlertAsync(
                    "File Picker Timeout",
                    "The file picker did not appear. On macOS it may open behind the app window. Try Command + Tab.",
                    "OK");

                return;
            }

            var image = await pickerTask;

            if (image == null)
            {
                AppendLog("User cancelled image selection");
                return;
            }

            AppendLog($"Selected file: {image.FileName}");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                await Shell.Current.DisplayAlertAsync
                (
                    "Invalid File",
                    "Please select a PNG or JPG image.",
                    "OK");

                return;
            }

            // Process the picked file in-memory (safe copy) and set ImageSource
            await ProcessPickedFile(image);
        }
        catch (Exception ex)
        {
            AppendLog($"Unhandled exception: {ex}");

            await Shell.Current.DisplayAlertAsync(
                "Error",
                ex.Message,
                "OK");
        }
    }

    public async Task InitializeAsync()
    {
        await LoadGenresAsync();
    }

    private async Task ProcessPickedFile(FileResult image)
    {
        try
        {
            using var inputStream = await image.OpenReadAsync();
            using var ms = new MemoryStream();
            await inputStream.CopyToAsync(ms);

            var bytes = ms.ToArray();
            const long maxBytes = 5 * 1024 * 1024;
            if (bytes.Length > maxBytes)
            {
                AppendLog($"Picked file too large: {bytes.Length} bytes");
                await Shell.Current.DisplayAlertAsync("File Too Large", "Please select an image smaller than 5 MB.", "OK");
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ImageSourceFile = ImageSource.FromStream(() => new MemoryStream(bytes));
                AppendLog("ImageSource set from memory");
                try
                {
                    // Store the image as a base64 string so it's available for the POST payload
                    AddProductModel.Image = Convert.ToBase64String(bytes);
                    AppendLog("AddProductModel.Image set (base64)");
                }
                catch (Exception ex)
                {
                    AppendLog($"Failed to set AddProductModel.Image: {ex}");
                }
            });
        }
        catch (Exception ex)
        {
            AppendLog($"ProcessPickedFile error: {ex}");
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    public async Task SaveGameData()
    {
        try
        {
            AppendLog("SaveGameData invoked");

            if (SelectedGenre == null)
            {
                MakeToast("Please select a genre.");
                return;
            }

            AddProductModel.GenreId = SelectedGenre.Id;

            // Ensure ReleasedDate is set on the model (DatePicker -> DateOnly)
            AddProductModel.ReleasedDate = DateOnly.FromDateTime(ReleaseDate);

            if (AddProductModel is null)
                return;

            var result = await productService.AddProductAsync(AddProductModel);

            if (result == null)
            {
                AppendLog("AddProductAsync returned null");
                MakeToast("Server error: no response");
                return;
            }

            MakeToast(result.Message ?? "Done");
            AppendLog($"SaveGameData result: Success={result.Success} Message={result.Message}");

            // Navigate back to homepage after successful save
            if (result.Success)
            {
                await Shell.Current.GoToAsync("///MainPage");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"SaveGameData exception: {ex}");
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
    private async Task LoadGenresAsync()
    {
        try
        {
            var genres = await productService.GetGenresAsync();
            if (genres == null)
                return;

            Genres.Clear();
            Genres.AddRange(genres);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async void MakeToast(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ToastDuration duration = ToastDuration.Long;
        double fontSize = 18;
        
        var toast = Toast.Make(message, duration, fontSize);
        
        await toast.Show(cancellationTokenSource.Token);
    }

}
