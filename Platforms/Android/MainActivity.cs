using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MisClaves;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Ignoramos el estado guardado para evitar que Android intente
        // reconstruir una vista de navegación que ya no existe (bug conocido
        // de restauración de Fragments en Shell de .NET MAUI).
        base.OnCreate(null);
    }
}