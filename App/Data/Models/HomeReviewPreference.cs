using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace App.Data.Models {
    [DebuggerDisplay("{Id}: {Title}")]
    public class HomeReviewPreference {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public int CategoryId { get; set; }
        public HomeReviewPreferenceCategory Category { get; set; }

        public List<HomeReviewUserPreference> Weights { get; set; }
        public List<HomeReviewHomePreference> Details { get; set; }
        public List<HomeReviewPreferenceLevel> Levels { get; set; }
        public List<HomeReviewHomePreferenceLevel> HomePreferenceLevels { get; set; }
    }
}
