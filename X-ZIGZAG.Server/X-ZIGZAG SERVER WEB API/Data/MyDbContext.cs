using Microsoft.EntityFrameworkCore;
using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.Data
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> op ):base(op) { }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SystemInfo> SystemsInfo { get; set; }
        public DbSet<CheckSetting> CheckSettings { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<X_ZIGZAG_SERVER_WEB_API.Models.Cookie> Cookies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instruction>()
                .HasKey(j => new { j.ClientId, j.InstructionId});
            modelBuilder.Entity<Result>()
               .HasKey(j => new { j.ClientId, j.InstructionId });
            modelBuilder.Entity<Password>()
              .HasKey(j => new { j.ClientId, j.PasswordId });
            modelBuilder.Entity<CreditCard>()
              .HasKey(j => new { j.ClientId, j.CreditCardId});
            modelBuilder.Entity<X_ZIGZAG_SERVER_WEB_API.Models.Cookie>()
              .HasKey(j => new { j.ClientId, j.CookieId });
        }
    }
}
