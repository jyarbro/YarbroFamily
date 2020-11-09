using App.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DbSet<Home> Homes { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Detail>()
                .HasOne(r => r.Category)
                .WithMany(r => r.Details)
                .IsRequired();

            builder.Entity<HomeDetail>()
                .HasOne(r => r.Home)
                .WithMany(r => r.HomeDetails)
                .IsRequired();
        }
    }
}
