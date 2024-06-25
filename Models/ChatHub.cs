using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CustomerWebSocket.Models
{
    public class ChatHub : Hub
    {
        private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("kN9Q5fy83Zw5e3tGj65KN5yHq+R1w9mqmpGYzQCUQQg=");

        public async Task SendMessage(string user, string message)
        {
            var encryptedMessage = EncryptMessage(message);
            await Clients.All.SendAsync("ReceiveMessage", user, encryptedMessage);
        }

        private string EncryptMessage(string message)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                aesAlg.IV = new byte[16]; // Initialization Vector (IV) for AES encryption

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(message);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptMessage(string encryptedMessage)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                aesAlg.IV = new byte[16]; // Assume IV is not used or is transmitted with the message

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
