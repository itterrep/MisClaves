// Models/Credential.cs
using SQLite;

namespace MisClaves.Models;

public class Credential
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Estos campos se guardan YA CIFRADOS (string en base64)
    public string UrlEnc { get; set; } = "";
    public string UsuarioEnc { get; set; } = "";
    public string PassEnc { get; set; } = "";
    public string ComentariosEnc { get; set; } = "";
}