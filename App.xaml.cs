using MisClaves.Services;
using MisClaves.Views;

namespace MisClaves;

public partial class App : Application
{
    private readonly MasterPasswordService _masterPasswordService;

    public App(MasterPasswordService masterPasswordService)
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
        _masterPasswordService = masterPasswordService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new LoginPage(_masterPasswordService)));
    }

    protected override void OnSleep()
    {
        _masterPasswordService.Lock();
    }

    protected override void OnResume()
    {
        if (!_masterPasswordService.IsUnlocked && Windows.Count > 0)
        {
            Windows[0].Page = new NavigationPage(new LoginPage(_masterPasswordService));
        }
    }
}