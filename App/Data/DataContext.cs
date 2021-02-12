using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Loggers;

namespace App.Data {
    public class DataContext : DbContext {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<HomeReviewFeature> HomeReviewFeatures { get; set; }
        public DbSet<HomeReviewFeatureChoice> HomeReviewFeatureChoices { get; set; }
        public DbSet<HomeReviewFeatureCategory> HomeReviewFeatureCategories { get; set; }
        public DbSet<HomeReviewUserWeight> HomeReviewUserWeights { get; set; }
        public DbSet<HomeReviewHome> HomeReviewHomes { get; set; }
        public DbSet<HomeReviewHomeFeature> HomeReviewHomeFeatures { get; set; }
        public DbSet<HomeReviewHomeFeatureChoice> HomeReviewHomeFeatureChoices { get; set; }
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

            builder.Entity<HomeReviewFeatureChoice>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.FeatureChoices);

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Home)
                .WithMany(o => o.HomeFeatures)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.HomeFeatures)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureChoice>()
                .HasOne(o => o.Home)
                .WithMany(o => o.HomeFeatureChoices)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureChoice>()
                .HasOne(o => o.Feature)
                .WithMany(o => o.HomeFeatureChoices)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureChoice>()
                .HasOne(o => o.FeatureChoice)
                .WithMany(o => o.HomeFeatureChoices)
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
                .HasOne(o => o.FeatureChoice)
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
