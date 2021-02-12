using System;

namespace App.Data.Models {
    public class HomeReviewHomeFeatureLevel {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public int HomeId { get; set; }
        public HomeReviewHome Home { get; set; }

        public int FeatureId { get; set; }
        public HomeReviewFeature Feature { get; set; }

        public int FeatureLevelId { get; set; }
        public HomeReviewFeatureLevel FeatureLevel { get; set; }
    }
}
