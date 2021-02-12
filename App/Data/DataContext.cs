using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Loggers;

namespace App.Data {
    public class DataContext : DbContext {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<HomeReviewFeature> HomeReviewFeatures { get; set; }
        public DbSet<HomeReviewFeatureLevel> HomeReviewFeatureLevels { get; set; }
        public DbSet<HomeReviewFeatureCategory> HomeReviewFeatureCategories { get; set; }
        public DbSet<HomeReviewUserWeight> HomeReviewUserWeights { get; set; }
        public DbSet<HomeReviewHome> HomeReviewHomes { get; set; }
        public DbSet<HomeReviewHomeFeature> HomeReviewHomeFeatures { get; set; }
        public DbSet<HomeReviewHomeFeatureLevel> HomeReviewHomeFeatureLevels { get; set; }
        public DbSet<HomeReviewLink> HomeReviewLinks { get; set; }
        public DbSet<HomeReviewBaseScoreModifier> HomeReviewBaseScoreModifiers { get; set; }
        public DbSet<LogEntry> YnabFeederLog { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<HomeReviewFeature>()
                .HasOne(o => o.FeatureCategory)
                .WithMany(o => o.Features)
                .IsRequired();

            builder.Entity<HomeReviewFeatureLevel>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.FeatureLevels);

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Home)
                .WithMany(o => o.HomeFeatures)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.HomeFeatures)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.Home)
                .WithMany(o => o.HomeFeatureLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.HomeFeatureLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.FeatureLevel)
                .WithMany(o => o.HomeFeatureLevels)
                .IsRequired();

            builder.Entity<HomeReviewLink>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Links)
                .IsRequired();

            builder.Entity<HomeReviewBaseScoreModifier>()
                .HasIndex(o => o.Type);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.UserWeights);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.FeatureLevel)
                .WithMany(o => o.UserWeights);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.User)
                .WithMany(o => o.UserWeights)
                .IsRequired();

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.CreatedBy);
        }
    }
}
