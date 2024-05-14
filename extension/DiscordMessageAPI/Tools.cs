using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordMessageAPI
{
    internal class Tools
    {
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string ExtFilePath = Path.Combine(AssemblyPath, "DiscordMessageAPI");
        private static readonly string LogFilePath = Path.Combine(ExtFilePath, "logs");
        private static readonly string Time = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss");
        static readonly string Webkey = GenerateRandomWebKey();
        private static readonly string LogFileName = Path.Combine(LogFilePath, $"{Time}.DiscordMessageAPI.log");

        internal static void Logger(Exception e = null, string s = "", bool loop = false)
        {
            try
            {
                if (!Directory.Exists(ExtFilePath))
                    Directory.CreateDirectory(ExtFilePath);
                if (!Directory.Exists(LogFilePath))
                    Directory.CreateDirectory(LogFilePath);

                using (StreamWriter file = new StreamWriter(@LogFileName, true))
                {
                    if (e != null)
                        s = $"{e}";
                    if (s.Length > 0)
                        file.WriteLine($"{DateTime.Now.ToString("T")} - {s}");
                }
            }
            catch (Exception i)
            {
                if (!loop)
                    Logger(i, null, true);
            };
        }
        internal static string GenTimeEncode()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes).Replace('"', '_');
            return id;
        }
        internal static string DecryptString(string cipherText, byte[] key)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(fullCipher, iv, iv.Length);

                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        internal static string EncryptString(string plainText, byte[] key)
        {
            // Generate a new AES object with a random IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Write the IV to the memory stream
                    msEncrypt.Write(iv, 0, iv.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }

                    byte[] encrypted = msEncrypt.ToArray();

                    // Return the encrypted bytes from the memory stream as a base64 encoded string
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
        internal static byte[] GenerateRandomWebKey()
        {
            string timeString = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(timeString));
                byte[] key = new byte[32];
                Array.Copy(hash, key, key.Length);
                return key;
            }
        }

        /*internal static async Task LogAsyncReply(HttpContent responseContent)
        {
            string readResponse = "";
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                readResponse += await reader.ReadToEndAsync();
            }
            if (readResponse.Length > 0) Logger(null, $"AsyncRet: {readResponse}");
        }*/
    }
}
