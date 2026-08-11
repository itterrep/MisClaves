using MisClaves.Views;

namespace MisClaves
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registramos la ruta "detalle" para poder navegar con Shell.Current.GoToAsync("detalle")
            Routing.RegisterRoute("detalle", typeof(DetalleCredencialPage));
        }
    }
}
