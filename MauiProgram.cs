using Microsoft.Extensions.Logging;
using MisClaves.Services;
using MisClaves.Data;
using MisClaves.Views;

namespace MisClaves
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
            builder.Services.AddSingleton<MasterPasswordService>();
            builder.Services.AddSingleton<CredentialRepository>();
            builder.Services.AddTransient<CredencialesPage>();
            builder.Services.AddTransient<Views.DetalleCredencialPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
