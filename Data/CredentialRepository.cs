// Data/CredentialRepository.cs
using SQLite;
using MisClaves.Models;
using MisClaves.Services;

namespace MisClaves.Data;

public class CredentialRepository
{
    private SQLiteAsyncConnection? _db;
    private readonly MasterPasswordService _masterPasswordService;

    public CredentialRepository(MasterPasswordService masterPasswordService)
    {
        _masterPasswordService = masterPasswordService;
    }

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db != null)
            return _db;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "misclaves.db3");
        _db = new SQLiteAsyncConnection(dbPath);
        await _db.CreateTableAsync<Credential>();
        return _db;
    }

    // Modelo "plano" que usa el resto de la app (pantallas), nunca toca el cifrado directamente
    public record CredentialDto(int Id, string Url, string Usuario, string Pass, string Comentarios);

    private byte[] GetSessionKeyOrThrow()
    {
        return _masterPasswordService.SessionKey
            ?? throw new InvalidOperationException("La app está bloqueada. No se puede acceder a los datos.");
    }

    public async Task<List<CredentialDto>> GetAllAsync()
    {
        var key = GetSessionKeyOrThrow();
        var db = await GetConnectionAsync();
        var items = await db.Table<Credential>().ToListAsync();

        return items.Select(c => new CredentialDto(
            c.Id,
            CryptoService.Decrypt(c.UrlEnc, key),
            CryptoService.Decrypt(c.UsuarioEnc, key),
            CryptoService.Decrypt(c.PassEnc, key),
            CryptoService.Decrypt(c.ComentariosEnc, key)
        )).ToList();
    }

    public async Task<int> AddAsync(string url, string usuario, string pass, string comentarios)
    {
        var key = GetSessionKeyOrThrow();
        var db = await GetConnectionAsync();

        var entity = new Credential
        {
            UrlEnc = CryptoService.Encrypt(url, key),
            UsuarioEnc = CryptoService.Encrypt(usuario, key),
            PassEnc = CryptoService.Encrypt(pass, key),
            ComentariosEnc = CryptoService.Encrypt(comentarios, key)
        };

        return await db.InsertAsync(entity);
    }

    public async Task UpdateAsync(int id, string url, string usuario, string pass, string comentarios)
    {
        var key = GetSessionKeyOrThrow();
        var db = await GetConnectionAsync();

        var entity = new Credential
        {
            Id = id,
            UrlEnc = CryptoService.Encrypt(url, key),
            UsuarioEnc = CryptoService.Encrypt(usuario, key),
            PassEnc = CryptoService.Encrypt(pass, key),
            ComentariosEnc = CryptoService.Encrypt(comentarios, key)
        };

        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAsync<Credential>(id);
    }
}