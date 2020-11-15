using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<DetailCategory> DetailCategories { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }
        public DbSet<HomeLink> HomeLinks { get; set; }
        public DbSet<SecurityRole> SecurityRoles { get; set; }
        public DbSet<UserSecurityRole> UserSecurityRoles { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Detail>()
                .HasOne(o => o.Category)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeDetail>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeDetail>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.HomeDetails)
                .IsRequired();

            builder.Entity<HomeLink>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Links)
                .IsRequired();

            builder.Entity<UserPreference>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Weights)
                .IsRequired();

            builder.Entity<UserPreference>()
                .HasOne(o => o.User)
                .WithMany(o => o.Preferences)
                .IsRequired();

            builder.Entity<UserPreference>()
                .HasOne(o => o.CreatedBy);

            builder.Entity<UserSecurityRole>()
                .HasOne(o => o.SecurityRole)
                .WithMany(o => o.Users)
                .IsRequired();

            builder.Entity<UserSecurityRole>()
                .HasOne(o => o.User)
                .WithMany(o => o.SecurityRoles)
                .IsRequired();
        }
    }
}
