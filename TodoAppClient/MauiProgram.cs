using Microsoft.Extensions.Logging;
using TodoAppClient.Services;
using TodoAppClient.ViewModels;
using TodoAppClient.Views;

namespace TodoAppClient;

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

		builder.Services.AddHttpClient<ITareaApi, TareaApi>(client =>
		{
			client.BaseAddress = new Uri(ApiConfig.BaseUrl);
			client.Timeout = TimeSpan.FromSeconds(15);
		});

		builder.Services.AddTransient<TareasListaViewModel>();
		builder.Services.AddTransient<TareaFormViewModel>();
		builder.Services.AddTransient<TareasListaPage>();
		builder.Services.AddTransient<TareaFormPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
