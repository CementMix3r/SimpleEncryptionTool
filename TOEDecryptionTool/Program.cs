using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace TOEDecryptionTool
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
        static void Main(string[] args)
        {
            string dir;

            do
            {
                dir = ValidateDirectory();
                Console.WriteLine("Directory to be decrypted: " + dir);

                if (Directory.GetFiles(dir, "*.enc").Any())
                {
                    string key = GetKey();
                    Config.SetKey(key);

                    string iv = GetIV();
                    Config.SetIV(iv);

                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    DecDir(dir);
                    sw.Stop();

                    int time = sw.Elapsed.Milliseconds;

                    Console.WriteLine($"Decryption Time: {time} ms");
                }
                else
                {
                    Console.WriteLine("No files with the .enc extension found in the directory!");
                }

            } while (!Directory.GetFiles(dir, "*.enc").Any());

            Console.ReadKey();
        }

        internal static string ValidateDirectory()
        {
            string dir;
            bool isValid = false;

            do
            {
                Console.WriteLine("Enter the directory you want to decrypt: ");
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

        private static string GetIV()
        {
            string iv;
            do
            {
                Console.WriteLine("Enter the IV you received from the bot: ");
                iv = Console.ReadLine();
                if (iv.Length != 16)
                {
                    Logger.LogError("IV must be 16 chars!");
                }
            } while (iv.Length != 16);

            return iv;
        }

        private static string GetKey()
        {
            string key;

            do
            {
                Console.WriteLine("Enter the key you used during encryption: ");
                key = Console.ReadLine();
                if (key.Length != 32)
                {
                    Logger.LogError("Key must be 32 chars !");
                }
            } while (key.Length != 32);

            return key;
        }

        private static void DelNote(string dir)
        {
            try
            {
                string path = Path.Combine(dir, "_Instructions.txt");

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        static void DecDir(string dir)
        {
            if (!IsFolderWhiteListed(dir))
            {
                DelNote(dir);

                try
                {
                    string[] files = Directory.GetFiles(dir);
                    List<string> subDirs = Directory.GetDirectories(dir).ToList();

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

                            if (file.ToLower().Contains(Config.ext.ToLower()))
                            {
                                CryptIO.DecryptFile(file);
                                Logger.Info("File Decrypted: " + file);
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

                    foreach (string subDir in subDirs)
                    {
                        if (!IsFolderWhiteListed(subDir))
                        {
                            DelNote(subDir);

                            try
                            {
                                DirectoryInfo dirInfo = new DirectoryInfo(subDir);
                                dirInfo.Attributes = FileAttributes.Normal;
                            }

                            catch (Exception e)
                            {
                                Logger.LogError("Could not set directory attributes: " + e.ToString());
                            }

                            DecDir(subDir);
                        }
                    }
                }

                catch
                {

                }
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
    }
}
