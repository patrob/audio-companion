using Microsoft.Extensions.Logging;

namespace AudioCompanion.App;

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
			});

		builder.Services.AddMauiBlazorWebView();

		// Register audio services - use real-time processor for better performance
		builder.Services.AddSingleton<AudioCompanion.Shared.Audio.IAudioProcessor, AudioCompanion.Shared.Audio.RealTimeAudioProcessor>();

#if MACCATALYST || MACOS
		builder.Services.AddSingleton<AudioCompanion.Shared.Audio.IAudioInputManager, AudioCompanion.App.Audio.MacAudioInputManager>();
#endif

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
