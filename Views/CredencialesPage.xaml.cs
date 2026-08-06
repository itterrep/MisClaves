using MisClaves.Data;

namespace MisClaves.Views;

public partial class CredencialesPage : ContentPage
{
    private readonly CredentialRepository _repository;

    public CredencialesPage(CredentialRepository repository)
    {
        InitializeComponent();
        _repository = repository;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarCredencialesAsync();
    }

    private async Task CargarCredencialesAsync()
    {
        var items = await _repository.GetAllAsync();
        CredencialesList.ItemsSource = items;
    }

    private async void OnCredencialTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is int id)
        {
            await Shell.Current.GoToAsync($"detalle?id={id}");
        }
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("detalle");
    }

    private async void OnAcercaDeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("acercade");
    }
}