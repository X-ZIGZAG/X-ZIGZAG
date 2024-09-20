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
            // UPLOAD AND DOWNLOAD
            cache.Set(2, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "CMD.txt")));
            cache.Set(3, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "POWERSHELL.txt")));
            // 4
            cache.Set(5, ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, "Scripts", "WIFI.txt")));
      
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
