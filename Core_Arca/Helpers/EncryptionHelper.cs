using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text;

namespace Core_Arca.Helpers
{
    public static class EncryptionHelper
    {
        public static string Encrypt(string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(Constant.ENC_KEY_TEXT);
            byte[] iv = Encoding.UTF8.GetBytes(Constant.ENC_IV_TEXT);
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] encryptedBytes;
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(password);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        private static string Decrypt(string encryptedText)
        {
            byte[] ciphertext = Convert.FromBase64String(encryptedText);
            byte[] key = Encoding.UTF8.GetBytes(Constant.ENC_KEY_TEXT);
            byte[] iv = Encoding.UTF8.GetBytes(Constant.ENC_IV_TEXT);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] decryptedBytes;
                using (var msDecrypt = new MemoryStream(ciphertext))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
