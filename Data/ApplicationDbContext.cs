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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerAccount>()
                .HasMany(a => a.Characters)
                .WithOne(c => c.PlayerAccount)
                .HasForeignKey(c => c.PlayerAccountID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

