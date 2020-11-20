using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<HomeReviewDetail> HomeReviewDetails { get; set; }
        public DbSet<HomeReviewDetailCategory> HomeReviewDetailCategories { get; set; }
        public DbSet<HomeReviewUserPreference> HomeReviewUserPreferences { get; set; }
        public DbSet<HomeReviewHome> HomeReviewHomes { get; set; }
        public DbSet<HomeReviewHomeDetail> HomeReviewHomeDetails { get; set; }
        public DbSet<HomeReviewLink> HomeReviewLinks { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<HomeReviewDetail>()
                .HasOne(o => o.Category)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomeDetail>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomeDetail>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewLink>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Links)
                .IsRequired();

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Weights)
                .IsRequired();

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.User)
                .WithMany(o => o.Preferences)
                .IsRequired();

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.CreatedBy);
        }
    }
}
