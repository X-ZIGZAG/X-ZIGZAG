using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;
namespace X_ZIGZAG_SERVER_WEB_API.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponseVM> Login(AdminVM authInfo);
       
    }
}
