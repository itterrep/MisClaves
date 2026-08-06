namespace MisClaves.Views;

public partial class AcercaDePage : ContentPage
{
    public AcercaDePage()
    {
        InitializeComponent();
        VersionLabel.Text = $"Versión {AppInfo.Current.VersionString}";
    }
}