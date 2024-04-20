using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TOEDecryptionTool
{
    internal class CryptIO
    {
        // Decrypt File
        internal static bool Decrypt(string file)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Key = Encoding.UTF8.GetBytes(Config.Key);
                    aes.IV = Encoding.UTF8.GetBytes(Config.IV);
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform d = aes.CreateDecryptor();

                    string denFile = file.Replace(Config.ext, String.Empty);

                    using (FileStream fsDe = new FileStream(file, FileMode.Open))
                    {
                        using (FileStream fsDecrypt = new FileStream(denFile, FileMode.Create))
                        {
                            using (CryptoStream cs = new CryptoStream(fsDe, d, CryptoStreamMode.Read))
                            {
                                int data;

                                while ((data = cs.ReadByte()) != -1)
                                {
                                    fsDecrypt.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[!] Could not decrypt file: " + e.ToString());
                return false;
            }

            return true;
        }

        // Handle Decrypt File
        internal static void DecryptFile(string file)
        {
            // If Dec function returns true then delete the original file
            if (Decrypt(file))
            {
                File.Delete(file);
            }
            else
            {
                Console.WriteLine($"[!] Failed to Decrypt File: {file}");
            }
        }
    }
}
