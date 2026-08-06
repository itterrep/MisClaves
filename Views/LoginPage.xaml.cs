using MisClaves.Services;

namespace MisClaves.Views;

public partial class LoginPage : ContentPage
{
    private readonly MasterPasswordService _masterPasswordService;
    private readonly bool _isFirstTime;

    public LoginPage(MasterPasswordService masterPasswordService)
    {
        InitializeComponent();
        _masterPasswordService = masterPasswordService;
        _isFirstTime = !_masterPasswordService.IsMasterPasswordSet();

        if (_isFirstTime)
        {
            TituloLabel.Text = "Configurá tu contraseña maestra";
            AccionButton.Text = "Crear contraseña";
            ConfirmPasswordEntry.IsVisible = true;
            VerConfirmPasswordButton.IsVisible = true; 
        }
        else
        {
            TituloLabel.Text = "Ingresá tu contraseña maestra";
            AccionButton.Text = "Desbloquear";
            ConfirmPasswordEntry.IsVisible = false;
            VerConfirmPasswordButton.IsVisible = false; 
        }
    }

    private async void OnAccionClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Ingresá una contraseña.");
            return;
        }

        if (_isFirstTime)
        {
            if (password.Length < 8)
            {
                MostrarError("Usá al menos 8 caracteres.");
                return;
            }

            if (password != ConfirmPasswordEntry.Text)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }

            _masterPasswordService.SetupMasterPassword(password);
            await IrAPantallaPrincipal();
        }
        else
        {
            bool ok = _masterPasswordService.TryUnlock(password);
            if (ok)
            {
                await IrAPantallaPrincipal();
            }
            else
            {
                MostrarError("Contraseña incorrecta.");
                PasswordEntry.Text = "";
            }
        }
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }

    private async Task IrAPantallaPrincipal()
    {
        if (Application.Current!.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AppShell();
        }
        await Task.CompletedTask;
    }

    private void OnVerPasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        VerPasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
    }

    private void OnVerConfirmPasswordClicked(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
        VerConfirmPasswordButton.Text = ConfirmPasswordEntry.IsPassword ? "👁" : "🙈";
    }

}