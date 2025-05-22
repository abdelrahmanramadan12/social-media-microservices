using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IEncryptionService
    {
        public string Encrypt(string plainText);

        public string Decrypt(string encryptedBase64);

    }
}


