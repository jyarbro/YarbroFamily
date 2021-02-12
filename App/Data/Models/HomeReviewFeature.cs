using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace App.Data.Models {
    [DebuggerDisplay("{Id}: {Title}")]
    public class HomeReviewFeature {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public int FeatureCategoryId { get; set; }
        public HomeReviewFeatureCategory FeatureCategory { get; set; }

        public List<HomeReviewFeatureChoice> FeatureChoices { get; set; }
        public List<HomeReviewHomeFeature> HomeFeatures { get; set; }
        public List<HomeReviewHomeFeatureChoice> HomeFeatureChoices { get; set; }
        public List<HomeReviewUserWeight> UserWeights { get; set; }
    }
}
