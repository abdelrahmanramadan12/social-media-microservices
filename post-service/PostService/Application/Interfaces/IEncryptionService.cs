namespace Application.IServices
{
    public interface IEncryptionService
    {
        public string Encrypt(string plainText);

        public string Decrypt(string encryptedBase64);

    }
}


