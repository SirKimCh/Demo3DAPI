using Microsoft.EntityFrameworkCore;
using Demo3DAPI.Models;

namespace Demo3DAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PlayerAccount> PlayerAccounts { get; set; }
        public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminRole = new Role { ID = 1, Name = "Admin" };
            var userRole = new Role { ID = 2, Name = "User" };
            modelBuilder.Entity<Role>().HasData(adminRole, userRole);

            var adminPassword = BCrypt.Net.BCrypt.HashPassword("abc@123");

            modelBuilder.Entity<PlayerAccount>().HasData(new PlayerAccount
            {
                ID = 1,
                UserName = "admin",
                Password = adminPassword,
                FullName = "Admin",
                RoleID = 1,
                PhoneNumber = null
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerAccount>(entity =>
            {
                entity.Property(a => a.RoleID).HasDefaultValue(2);

                entity.HasMany(a => a.Characters)
                    .WithOne(c => c.PlayerAccount)
                    .HasForeignKey(c => c.PlayerAccountID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Role)
                    .WithMany(r => r.PlayerAccounts)
                    .HasForeignKey(a => a.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

