using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GazRouter.Service.Exchange.Lib.Cryptography
{
    public static class Cryptoghraphy
    {
        private static readonly string Password = @"V%9JWwjr";


        public static string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("No data given");
            var rawData = Encoding.Unicode.GetBytes(data);

            var encrypted = Encrypt(rawData);
            return Encoding.Unicode.GetString(encrypted);
        }

        public static byte[] Encrypt(byte[] data)
        {
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentException("No password given");

            // setup the encryption algorithm
            using (var keyGenerator = new Rfc2898DeriveBytes(Password, 8))
            {
                var aes = Rijndael.Create();
                aes.IV = keyGenerator.GetBytes(aes.BlockSize / 8);
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);

                // encrypt the data
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    memoryStream.Write(keyGenerator.Salt, 0, keyGenerator.Salt.Length);
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.Close();

                    return memoryStream.ToArray();
                }
            }
        }

        public static string Decrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("No data given");
            var rawData = Encoding.Unicode.GetBytes(data);

            return Encoding.Unicode.GetString(rawData);
        }


        public static byte[] Decrypt(byte[] data)
        {
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentException("No password given");

            if (data.Length < 8)
                throw new ArgumentException("Invalid input data");

            // setup the decryption algorithm
            var salt = new byte[8];
            for (var i = 0; i < salt.Length; i++)
                salt[i] = data[i];

            using (var keyGenerator = new Rfc2898DeriveBytes(Password, salt))
            {
                var aes = Rijndael.Create();
                aes.IV = keyGenerator.GetBytes(aes.BlockSize / 8);
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);

                // decrypt the data
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 8, data.Length - 8);
                    cryptoStream.Close();

                    return memoryStream.ToArray();
                }
            }
        }
    }
}