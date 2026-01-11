using System;
using Client.Models;
using Client.Services;
using GameLibrary.Models;
using MvvmHelpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Client.ViewModels;

public partial class PhoneHomePageViewModel: BaseViewModel
{
    private readonly IProductService productService;
    public ObservableRangeCollection<ProductTem> Games { get; set; } = new();

    public PhoneHomePageViewModel(IProductService productService)
    {
        this.productService = productService;
        Title = "Game Library";
        GetProducts();

        // Refresh the list when a game is deleted elsewhere (subscribe with WeakReferenceMessenger)
        try
        {
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<PhoneHomePageViewModel, Client.Messages.GameDeletedMessage, string>(this, "GameDeleted", (r, m) =>
            {
                var id = m.Value;
                System.Diagnostics.Debug.WriteLine($"Received GameDeleted for id={id}, reloading list");
                GetProducts();
            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task RefreshGames()
    {
        GetProducts();
    }

    private async void GetProducts()
    {
        try
        {
            var products = await productService.GetProductAsync();
            System.Diagnostics.Debug.WriteLine($"GetProducts: fetched {products?.Count ?? 0} items");
            if (products == null || products.Count == 0)
                return;

            var tempList = new List<ProductTem>();

            foreach (var product in products)
            {
                var newGames = new ProductTem();

                newGames.Id = product.Id;
                newGames.Name = product.Name;
                newGames.Description = product.Description;
                newGames.Price = product.Price;
                newGames.ReleasedDate = product.ReleasedDate;
                newGames.GenreId = product.GenreId;
                newGames.GenreName = product.Genre?.Name ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(product.Image))
                {
                    try
                    {
                        var img = Convert.FromBase64String(product.Image);
                        // Keep base64 for edits
                        newGames.ImageBase64 = product.Image;
                        // Create the stream lazily so each ImageSource gets its own stream
                        newGames.Image = ImageSource.FromStream(() => new MemoryStream(img));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Image decode failed for product {product.Id}: {ex}");
                        newGames.ImageBase64 = null;
                        newGames.Image = null;
                    }
                }
                else
                {
                    newGames.ImageBase64 = null;
                    newGames.Image = null;
                }

                tempList.Add(newGames);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Games.Clear();
                Games.AddRange(tempList);
                System.Diagnostics.Debug.WriteLine($"GetProducts: added {tempList.Count} items to Games collection");
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetProducts exception: {ex}");
        }
    }
}
