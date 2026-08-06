using MisClaves.Data;

namespace MisClaves.Views;

[QueryProperty(nameof(Id), "id")]
public partial class DetalleCredencialPage : ContentPage
{
    private readonly CredentialRepository _repository;
    private int? _id; // null = credencial nueva, con valor = edición

    public string Id
    {
        set => _id = int.TryParse(value, out int id) ? id : null;
    }

    public DetalleCredencialPage(CredentialRepository repository)
    {
        InitializeComponent();
        _repository = repository;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_id.HasValue)
        {
            Title = "Editar credencial";
            EliminarButton.IsVisible = true;
            await CargarCredencialAsync(_id.Value);
        }
        else
        {
            Title = "Nueva credencial";
            EliminarButton.IsVisible = false;
        }
    }

    private async Task CargarCredencialAsync(int id)
    {
        var items = await _repository.GetAllAsync();
        var item = items.FirstOrDefault(c => c.Id == id);

        if (item != null)
        {
            UrlEntry.Text = item.Url;
            UsuarioEntry.Text = item.Usuario;
            PassEntry.Text = item.Pass;
            ComentariosEditor.Text = item.Comentarios;
        }
    }

    private void OnVerPassClicked(object sender, EventArgs e)
    {
        PassEntry.IsPassword = !PassEntry.IsPassword;
        VerPassButton.Text = PassEntry.IsPassword ? "👁" : "🙈";
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string url = UrlEntry.Text ?? "";
        string usuario = UsuarioEntry.Text ?? "";
        string pass = PassEntry.Text ?? "";
        string comentarios = ComentariosEditor.Text ?? "";

        if (string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(usuario))
        {
            await DisplayAlert("Falta información", "Completá al menos la URL o el usuario.", "OK");
            return;
        }

        if (_id.HasValue)
        {
            await _repository.UpdateAsync(_id.Value, url, usuario, pass, comentarios);
        }
        else
        {
            await _repository.AddAsync(url, usuario, pass, comentarios);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Eliminar", "¿Seguro que querés eliminar esta credencial?", "Sí", "Cancelar");
        if (confirmar && _id.HasValue)
        {
            await _repository.DeleteAsync(_id.Value);
            await Shell.Current.GoToAsync("..");
        }
    }
}