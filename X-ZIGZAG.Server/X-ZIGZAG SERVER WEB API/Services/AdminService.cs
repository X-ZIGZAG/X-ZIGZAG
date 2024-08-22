using Microsoft.EntityFrameworkCore;
using X_ZIGZAG_SERVER_WEB_API.Models;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.Data;

namespace X_ZIGZAG_SERVER_WEB_API.Services
{
    using BCrypt.Net;
    using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
    using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

    public class AdminService:IAdminService
    {
        private readonly MyDbContext _context;
        public AdminService(MyDbContext context)
        {
            _context = context;
        }
        public async Task<AdminResponseVM> GetAll()
        {
            return new AdminResponseVM { Admins = await _context.Admins.Select(user => user.Username).ToListAsync() }; ;
        }

        public async Task<AdminResponseVM> Create(AdminVM authInfo)
        {
            
            var AlreadyExist = await _context.Admins.AnyAsync(u => u.Username.Equals(authInfo.Username));
            if (AlreadyExist)
            {
                return new AdminResponseVM { Message = "A User with this username already exists." };
            }
            var newAdmin = new Admin
            {
                Id = Guid.NewGuid().ToString(),
                Username = authInfo.Username,
                Password = BCrypt.HashPassword(authInfo.Password)
            };
            await _context.AddAsync(newAdmin);
            await _context.SaveChangesAsync();
            return new AdminResponseVM { Token = newAdmin.Id };

        }
        public async Task<AdminResponseVM> UpdateUsername(string username,string newUsername)
        {
            var user = await _context.Admins.Where(u => u.Username.Equals(username)).FirstOrDefaultAsync();
            if (user == null)
            {
                return new AdminResponseVM { Message = "A User with this username doesn't exists." };
            }
            var anotherUserExist = await _context.Admins.AnyAsync(u => u.Username.Equals(username));
            if (anotherUserExist)
            {
                return new AdminResponseVM { Message = "Another User with this username exists." };

            }
            user.Username = newUsername;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return new AdminResponseVM {};

        }
        public async Task<AdminResponseVM> UpdatePassword(string username, string password)
        {
            var user = await _context.Admins.Where(u => u.Username.Equals(username)).FirstOrDefaultAsync();
            if (user == null)
            {
                return new AdminResponseVM { Message = "A User with this username doesn't exists." };
            }
            user.Password = BCrypt.HashPassword(password);
            _context.Update(user);
            await _context.SaveChangesAsync();
            return new AdminResponseVM { };

        }
        public async Task<AdminResponseVM> Delete(string username)
        {
            var user = await _context.Admins.Where(u=> u.Username.Equals(username)).FirstOrDefaultAsync();

            if (user == null)
            {
                
                return new AdminResponseVM { Message="User Not Found" };
            }
            _context.Admins.Remove(user);
            await _context.SaveChangesAsync();
            return new AdminResponseVM {};

        }
    }
}
