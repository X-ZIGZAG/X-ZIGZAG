using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.Data
{
    using BCrypt.Net;
    using System;

    public class DbInitializer
    {
        public static void Initialize(MyDbContext context)
        {
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
    }
}
