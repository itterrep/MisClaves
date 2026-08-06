// Services/MasterPasswordService.cs
using System.Security.Cryptography;

namespace MisClaves.Services;

public class MasterPasswordService
{
    private const string SaltKey = "master_salt";
    private const string VerifierKey = "master_verifier";
    private const string VerifierPlainText = "MISCLAVES_OK_v1";

    // Clave derivada en memoria mientras la app está desbloqueada. NUNCA se persiste.
    public byte[]? SessionKey { get; private set; }

    public bool IsMasterPasswordSet()
    {
        return Preferences.Default.ContainsKey(SaltKey);
    }

    // Se llama UNA sola vez, la primera vez que se abre la app
    public void SetupMasterPassword(string masterPassword)
    {
        byte[] salt = CryptoService.GenerateSalt();
        byte[] key = CryptoService.DeriveKey(masterPassword, salt);
        string verifier = CryptoService.Encrypt(VerifierPlainText, key);

        // El salt y el verificador cifrado NO son secretos por sí solos, se pueden guardar en Preferences
        Preferences.Default.Set(SaltKey, Convert.ToBase64String(salt));
        Preferences.Default.Set(VerifierKey, verifier);

        SessionKey = key; // queda "desbloqueada" tras configurar
    }

    // Intenta desbloquear con la contraseña ingresada. Devuelve true/false.
    public bool TryUnlock(string masterPassword)
    {
        try
        {
            byte[] salt = Convert.FromBase64String(Preferences.Default.Get(SaltKey, ""));
            string verifier = Preferences.Default.Get(VerifierKey, "");

            byte[] key = CryptoService.DeriveKey(masterPassword, salt);
            string decrypted = CryptoService.Decrypt(verifier, key); // falla si la clave está mal

            if (decrypted == VerifierPlainText)
            {
                SessionKey = key;
                return true;
            }
            return false;
        }
        catch (CryptographicException)
        {
            return false; // contraseña incorrecta
        }
    }

    // Bloquea la app: borra la clave de memoria
    public void Lock()
    {
        if (SessionKey != null)
        {
            Array.Clear(SessionKey, 0, SessionKey.Length); // borrado explícito de la RAM
            SessionKey = null;
        }
    }

    public bool IsUnlocked => SessionKey != null;
}