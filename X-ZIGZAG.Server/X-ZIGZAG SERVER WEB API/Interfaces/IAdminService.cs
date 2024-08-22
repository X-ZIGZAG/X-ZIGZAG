using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Interfaces
{
    public interface IAdminService
    {
        public Task<AdminResponseVM> Create(AdminVM authInfo);
        public Task<AdminResponseVM> GetAll();
        public Task<AdminResponseVM> UpdateUsername(string username,string newUsername);
        public Task<AdminResponseVM> UpdatePassword(string username,string password);
        public Task<AdminResponseVM> Delete(string username);
    }
}
