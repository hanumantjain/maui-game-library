using Client.Services;
using Client.ViewModels;
using Client.Views.Desktop;
using Client.Views.Phone;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureSyncfusionCore()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<DesktopHomePageViewModel>();
		builder.Services.AddSingleton<DesktopHomePage>();

		builder.Services.AddSingleton<AddProductPageViewModel>();
		builder.Services.AddSingleton<AddProductPage>();
		builder.Services.AddSingleton<PhoneAddProductPage>();

		builder.Services.AddSingleton<PhoneHomePageViewModel>();
		builder.Services.AddSingleton<PhoneHomePage>();
		builder.Services.AddSingleton<PhoneGameDetailPage>();
		builder.Services.AddSingleton<PhoneEditGamePage>();

		builder.Services.AddHttpClient<IProductService, ProductService>();


#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		// Expose DI container on App for simple service lookup in views
		if (app.Services.GetService<App>() is App application)
		{
			application.Services = app.Services;
		}

		return app;
	}
}
