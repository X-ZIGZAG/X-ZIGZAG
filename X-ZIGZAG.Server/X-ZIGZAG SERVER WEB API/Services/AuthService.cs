using X_ZIGZAG_SERVER_WEB_API.Data;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;

namespace X_ZIGZAG_SERVER_WEB_API.Services
{
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;
    using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
    using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

    public class AuthService : IAuthService
    {
        private readonly MyDbContext _context;
        public AuthService(MyDbContext context) {
            _context = context;
        }
        
        public async Task<LoginResponseVM> Login(AdminVM authInfo)
        {
            var adminInfo = await _context.Admins.Where(user => user.Username.Equals(authInfo.Username)).FirstOrDefaultAsync();
            if (adminInfo != null && BCrypt.Verify(authInfo.Password, adminInfo.Password))
            {
                _context.Admins.Remove(adminInfo);
                await _context.SaveChangesAsync();

                adminInfo.Id=Guid.NewGuid().ToString();
                await _context.Admins.AddAsync(adminInfo);
                await _context.SaveChangesAsync();
                return new LoginResponseVM { Token = adminInfo.Id };
            }
            return new LoginResponseVM { Message = "Invalid Credentials" };
        }
      
    }
}
