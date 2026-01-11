using Microsoft.Extensions.DependencyInjection;

namespace Client;

public partial class App : Application
{
	public IServiceProvider? Services { get; internal set; }

	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}