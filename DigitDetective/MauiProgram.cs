using Microsoft.Extensions.Logging;

namespace DigitDetective
{
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
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LeaderboardPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AchievementsPage>();
            builder.Services.AddSingleton<ProgressionService>();
            builder.Services.AddSingleton<AudioService>();
            builder.Services.AddSingleton<LeaderboardService>();
            builder.Services.AddSingleton<AchievementService>();

            return builder.Build();
        }
    }
}
