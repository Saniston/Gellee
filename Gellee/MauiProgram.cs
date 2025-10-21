using Gellee.Services;
using Gellee.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace Gellee
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddSingleton(new DatabaseService());
            builder.Services.AddSingleton<UnitOfMeasurementService>();

            // Registrar pages para que possam ser resolvidas via DI quando navegadas por rota
            builder.Services.AddTransient<Pages.Units.UnitsPage>();

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

            return builder.Build();
        }
    }
}
