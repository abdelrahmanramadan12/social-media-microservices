using Application.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const string Utf8Key = "SometimesILoveC#";

        public string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(Utf8Key);
            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();

            using MemoryStream ms = new();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter writer = new(cs);
            writer.Write(plainText);

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string encryptedBase64)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedBase64);
            byte[] keyBytes = Encoding.UTF8.GetBytes(Utf8Key);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;

            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using MemoryStream ms = new(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cs);
            return reader.ReadToEnd();
        }

    }
}
