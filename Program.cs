using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HitmanLauncherWithConfig
{
    // Represents the settings read from config/options.json
    class Options
    {
        [JsonPropertyName("PeacockFolder")]
        public string PeacockFolder { get; set; }

        [JsonPropertyName("PatcherExe")]
        public string PatcherExe { get; set; }

        [JsonPropertyName("ServerCmd")]
        public string ServerCmd { get; set; }

        [JsonPropertyName("ServerWindowTitle")]
        public string ServerWindowTitle { get; set; }

        [JsonPropertyName("EpicUri")]
        public string EpicUri { get; set; }

        [JsonPropertyName("CloseDelaySeconds")]
        public int CloseDelaySeconds { get; set; }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // -------------------------
            // Determine config folder & file in a subfolder "config"
            // -------------------------
            string exeFolder = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string configFolder = Path.Combine(exeFolder, "config");
            string configPath = Path.Combine(configFolder, "options.json");

            // Ensure the config folder exists
            if (!Directory.Exists(configFolder))
            {
                try
                {
                    Directory.CreateDirectory(configFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating config folder at {configFolder}: {ex.Message}");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }
            }

            Options options = null;

            // First-run: generate default options if missing
            if (!File.Exists(configPath))
            {
                options = new Options
                {
                    PeacockFolder     = @"PATH TO PEACOCK",
                    PatcherExe        = "NAME OF PEACOCK PATCHER EXE FILE",
                    ServerCmd         = "NAME OF PEACOCK SERVER CMD FILE",
                    ServerWindowTitle = "Hitman Local Server",
                    EpicUri           = "com.epicgames.launcher://apps/ed55aa5edc5941de92fd7f64de415793%3A7d9ce7dd6f2e4ee98a55bd50a9bb78e0%3AEider?action=launch&silent=true",
                    CloseDelaySeconds = 5
                };
                try
                {
                    var optionsJson = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(configPath, optionsJson);
                    File.WriteAllText("config/README.md", "# PLEASE KEEP IN MIND:\n- All directory paths in options.json must use DOUBLE BACKSLASHES (e.g., C:\\\\Path\\\\To\\\\Peacock)\n- Do not remove any required fields.\n");
                    Console.WriteLine("No configuration file found.");
                    Console.WriteLine($"A default configuration has been generated at:\n  {configPath}");
                    Console.WriteLine("Please edit this file with correct paths/settings, then run this launcher again.");
                    Console.WriteLine("This window will close automatically in 15 seconds...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating default config file at {configPath}: {ex.Message}");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }
                await Task.Delay(TimeSpan.FromSeconds(15));
                return 0;
            }

            // -------------------------
            // Read existing options.json
            // -------------------------
            try
            {
                string jsonText = File.ReadAllText(configPath);
                options = JsonSerializer.Deserialize<Options>(jsonText);
                if (options == null)
                {
                    Console.WriteLine($"Failed to parse configuration (null) from {configPath}.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing configuration file {configPath}: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 1;
            }

            // -------------------------
            // Validate required fields
            // -------------------------
            if (string.IsNullOrWhiteSpace(options.PeacockFolder) ||
                string.IsNullOrWhiteSpace(options.PatcherExe) ||
                string.IsNullOrWhiteSpace(options.ServerCmd) ||
                string.IsNullOrWhiteSpace(options.ServerWindowTitle) ||
                string.IsNullOrWhiteSpace(options.EpicUri) ||
                options.CloseDelaySeconds < 0)
            {
                Console.WriteLine("Configuration contains invalid or missing fields. Please check config/options.json.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 1;
            }

            // Build full paths from options
            string peacockFolder = options.PeacockFolder;
            string patcherPath   = Path.Combine(peacockFolder, options.PatcherExe);
            string serverCmdPath = Path.Combine(peacockFolder, options.ServerCmd);
            string epicUri       = options.EpicUri;
            string serverTitle   = options.ServerWindowTitle;
            int closeDelay       = options.CloseDelaySeconds;

            // Echo loaded configuration
            Console.WriteLine("Configuration loaded from config/options.json:");
            Console.WriteLine($"  PeacockFolder     = {peacockFolder}");
            Console.WriteLine($"  PatcherExe        = {options.PatcherExe}");
            Console.WriteLine($"  ServerCmd         = {options.ServerCmd}");
            Console.WriteLine($"  ServerWindowTitle = {serverTitle}");
            Console.WriteLine($"  EpicUri           = {epicUri}");
            Console.WriteLine($"  CloseDelaySeconds = {closeDelay}");
            Console.WriteLine();

            // -------------------------
            // 1. Launch PeacockPatcher.exe if it exists
            // -------------------------
            if (File.Exists(patcherPath))
            {
                try
                {
                    Console.WriteLine($"Starting PeacockPatcher: {patcherPath}");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName         = patcherPath,
                        WorkingDirectory = peacockFolder,
                        UseShellExecute  = true
                    });
                    // Optional delay if the patcher needs time to initialize
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error launching Peacock Patcher: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Peacock Patcher file not found at: {patcherPath}");
            }

            // -------------------------
            // 2. Launch Start Server.cmd in separate window
            // -------------------------
            if (File.Exists(serverCmdPath))
            {
                try
                {
                    Console.WriteLine($"Launching server script in new window titled \"{serverTitle}\": {serverCmdPath}");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName         = "cmd.exe",
                        Arguments        = $"/c start \"{serverTitle}\" \"{serverCmdPath}\"",
                        WorkingDirectory = peacockFolder,
                        UseShellExecute  = false,
                        CreateNoWindow   = true
                    });
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error launching server script: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Start Server.cmd not found at: {serverCmdPath}");
            }

            // -------------------------
            // 3. Launch Hitman via Epic URI
            // -------------------------
            try
            {
                Console.WriteLine("Launching Hitman via Epic Games Launcher shortcut...");
                Process.Start(new ProcessStartInfo
                {
                    FileName         = "cmd.exe",
                    Arguments        = $"/c start \"\" \"{epicUri}\"",
                    UseShellExecute  = false,
                    CreateNoWindow   = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Hitman via Epic shortcut: {ex.Message}");
            }

            // -------------------------
            // 4. Wait configured seconds then exit (closing console if double-clicked)
            // -------------------------
            if (closeDelay > 0)
            {
                Console.WriteLine($"Waiting {closeDelay} second(s) before closing launcher window...");
                await Task.Delay(TimeSpan.FromSeconds(closeDelay));
            }

            return 0;
        }
    }
}
