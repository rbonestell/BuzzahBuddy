using Microsoft.Extensions.Logging;
using BuzzahBuddy.Services.Bluetooth;
using BuzzahBuddy.Services.Glove;
using BuzzahBuddy.Services.Storage;
using BuzzahBuddy.ViewModels;
using BuzzahBuddy.Views;

namespace BuzzahBuddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register Services
		builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
		builder.Services.AddSingleton<IGloveControlService, GloveControlService>();
		builder.Services.AddSingleton<IDataStorageService, PreferencesStorageService>();

		// Register ViewModels
		builder.Services.AddTransient<MainPageViewModel>();
		builder.Services.AddTransient<DeviceListViewModel>();
		builder.Services.AddTransient<GloveControlViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Register Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<DeviceListPage>();
		builder.Services.AddTransient<GloveControlPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder.Build();
	}
}
