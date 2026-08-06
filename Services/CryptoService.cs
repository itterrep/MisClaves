// Services/CryptoService.cs
using System.Security.Cryptography;
using System.Text;

namespace MisClaves.Services;

public static class CryptoService
{
    private const int SaltSize = 16;
    private const int KeySize = 32; // AES-256
    private const int Iterations = 200_000; // PBKDF2, cuanto más alto, más lento para un atacante
    private const int NonceSize = 12; // AES-GCM
    private const int TagSize = 16;

    public static byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    // Deriva la clave AES a partir de la contraseña maestra + salt
    public static byte[] DeriveKey(string masterPassword, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(masterPassword),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }

    // Cifra un texto plano con la clave derivada -> devuelve string en base64 (nonce + tag + cipher)
    public static string Encrypt(string plainText, byte[] key)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // Concatenamos: nonce + tag + cifrado, y lo pasamos a base64 para guardarlo como texto
        byte[] result = new byte[NonceSize + TagSize + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
        Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    // Descifra; si la clave es incorrecta, AesGcm tira una excepción (CryptographicException)
    public static string Decrypt(string base64Data, byte[] key)
    {
        byte[] data = Convert.FromBase64String(base64Data);

        byte[] nonce = data[..NonceSize];
        byte[] tag = data[NonceSize..(NonceSize + TagSize)];
        byte[] cipherBytes = data[(NonceSize + TagSize)..];
        byte[] plainBytes = new byte[cipherBytes.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes); // lanza excepción si la clave está mal

        return Encoding.UTF8.GetString(plainBytes);
    }
}