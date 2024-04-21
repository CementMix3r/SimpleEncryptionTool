using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TOEDecryptionTool
{
    internal class Config
    {
        // internal static readonly string IV = Byte2ToStr(IVGen());
        internal static readonly string ext = ".enc";
        private static string _key;
        private static string _IV;
        internal static string IV
        {
            get { return _IV; }
        }
        
        internal static string Key
        {
            get { return _key; }
        }


        internal static void SetKey(string key)
        {
            if (key.Length == 32)
            {
                _key = key;
            }
            else
            {
                Console.WriteLine("Key must be 32 chars !");
            }
        }

        internal static void SetIV(string iv)
        {
            if (iv.Length == 16)
            {
                _IV = iv;
            }
            else
            {
                Console.WriteLine("IV must be 16 chars !");
            }
        }

        internal static readonly string[] WhiteFldrs =
        {
            "\\AppData", "\\ApplicationData", "\\Windows", "\\Program Files\\",
            "\\Program Files (x86)\\", "\\recycle.bin", "\\config.msi",
            "\\$windows.~bt", "\\$windows.~ws\\", "\\Windows", "\\boot",
            "\\program files\\", "\\program files (x86)\\", "\\ProgramData",
            "\\System Volume Information\\", "Tor Browser\\", "\\windows.old",
            "\\intel", "\\msocache", "\\perflogs", "\\x64dbg", "\\Public",
            "\\All Users\\", "\\Default", "\\Microsoft", "\\$Recycle.Bin",
            "\\.", "\\$WinREAgent", "\\hp", "\\SWSetup", "\\System.sav", "\\go",
            "\\Boot", "\\winnt", "\\Roaming"
        };

        internal static byte[] IVGen()
        {
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[8];
                rngCsp.GetBytes(iv);
                return iv;
            }
        }


        internal static string Byte2ToStr(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
