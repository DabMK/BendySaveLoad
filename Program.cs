using System.Diagnostics;

namespace BendySaveLoad
{
    internal class Program
    {
        private static string temp = Path.GetTempPath();
        private static bool debug = false;
        private static string gameFiles = string.Empty;

        private static void Main(string[] args)
        {
            // Check if launched in debug mode
            if (args.Length == 1 && args[0].ToString() == "1") { debug = true; }

            // Check if Bendy is installed
            if (!Checker.IsGameInstalled())
            {
                Console.WriteLine($"Bendy and the Ink Machine is not installed in this local disk {Path.GetPathRoot(Environment.SystemDirectory)}");
                Console.ReadKey();
                Environment.Exit(0);
            }
            // Check if game was ever played
            if (!Checker.WasGamePlayed())
            {
                Console.WriteLine($"You have never started a game of Bendy and the Ink Machine");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Initialize
            gameFiles = Checker.gameFiles;
            if (!Directory.Exists(@$"{gameFiles}\Backups"))
            {
                Directory.CreateDirectory(@$"{gameFiles}\Backups");
            }

            // Load the menu
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===============================================");
                Console.WriteLine("BENDY AND THE INK MACHINE SAVE/LOAD");
                Console.WriteLine("===============================================");
                Console.Write(Environment.NewLine);
                Console.WriteLine("1) Save Current State");
                Console.WriteLine("2) Load a Saved State");
                Console.WriteLine("3) Exit" + Environment.NewLine);
                Console.Write("> ");
                ConsoleKey input = Console.ReadKey().Key;
                Console.Write(Environment.NewLine);
                switch (input)
                {
                    case ConsoleKey.D1: // Save

                        string name = string.Empty;
                        while (string.IsNullOrWhiteSpace(name) || name.Contains('\\') || name.Contains('/') || name.Contains(':') || name.Contains('*') || name.Contains('?') || name.Contains('"') || name.Contains('<') || name.Contains('>') || name.Contains('|') || DoesSaveExist(name))
                        {
                            Console.Write("Name of the save: ");
                            name = Console.ReadLine();
                        }
                        try
                        {
                            File.Copy(@$"{gameFiles}\batim.game", @$"{gameFiles}\Backups\{name}.game");
                        }
                        catch
                        {
                            Done($"Couldn't save game \"{name}\"! Make sure you are not in game! Press any button to return to main menu");
                            continue;
                        }
                        Done($"Game saved as \"{name}\"! Press any button to return to main menu");

                        continue;
                    case ConsoleKey.D2: // Load

                        string[] files = GetSaves();
                        Console.WriteLine("Saves:");
                        for (int i = 0; i < files.Length; i++)
                        {
                            Console.WriteLine(@$"{i + 1}) {GetFileName(files, i)}");
                        }
                        string load = string.Empty;
                        Console.Write(Environment.NewLine);
                        while (string.IsNullOrWhiteSpace(load) || !int.TryParse(load, out _) || int.Parse(load) > files.Length || int.Parse(load) < 1)
                        {
                            Console.Write("Number of File to Load: ");
                            load = Console.ReadLine();
                        }
                        string loadName = GetFileName(files, int.Parse(load) - 1);
                        if (File.Exists(@$"{temp}\batim.game")) { File.Delete(@$"{temp}\batim.game"); }
                        try
                        {
                            File.Move(@$"{gameFiles}\batim.game", @$"{temp}\batim.game");
                        }
                        catch (Exception ex)
                        {
                            if (debug) { Console.WriteLine(ex); }
                            Done($"Couldn't load game \"{loadName}\"! Make sure you are not in game! Press any button to return to main menu");
                            continue;
                        }
                        try
                        {
                            File.Copy(@$"{gameFiles}\Backups\{files[int.Parse(load) - 1]}", @$"{gameFiles}\batim.game");
                        }
                        catch (Exception ex)
                        {
                            File.Move(@$"{temp}\batim.game", @$"{gameFiles}\batim.game");
                            if (debug) { Console.WriteLine(ex); }
                            Done($"Couldn't load game \"{loadName}\"! Make sure you are not in game! Press any button to return to main menu");
                            continue;
                        }
                        Done($"File loaded: \"{loadName}\"! Press any button to restart the game and return to main menu");
                        Restart();
                        continue;
                    case ConsoleKey.D3: // Exit
                        Environment.Exit(0);
                        continue;
                    default: continue;
                }
            }
        }

        private static string[] GetSaves()
        {
            string[] tmpFiles = Directory.GetFiles(@$"{gameFiles}\Backups");
            string[] files = new string[tmpFiles.Length];
            for (int i = 0; i < tmpFiles.Length; i++)
            {
                files[i] = Path.GetFileName(tmpFiles[i]);
            }
            return files;
        }

        private static string GetFileName(string[] files, int index)
        {
            return files[index][..files[index].LastIndexOf('.')];
        }

        private static bool DoesSaveExist(string name)
        {
            string[] saves = GetSaves();
            for (int i = 0; i < saves.Length; i++)
            {
                if (name.Equals(GetFileName(saves, i), StringComparison.CurrentCultureIgnoreCase)) { return true; }
            }
            return false;
        }

        private static void Restart()
        {
            string processName = "Bendy and the Ink Machine";
            // Kill
            Process[] process = Process.GetProcessesByName(processName);
            foreach (Process proc in process)
            {
                proc.Kill();
            }
            Thread.Sleep(500); // Pause

            // Start
            string executable = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\Steam\steamapps\common\{processName}\{processName}.exe";
            if (File.Exists(executable)) { Process.Start(executable); }
        }

        private static void Done(string msg)
        {
            Console.Clear();
            Console.Write(msg);
            Console.ReadKey();
        }
    }
}