using System;
using System.Collections.Generic;

namespace App.Data.Models {
    public class HomeReviewPreferenceLevel {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public int PreferenceId { get; set; }
        public HomeReviewPreference Preference { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<HomeReviewUserPreference> Weights { get; set; }
        public List<HomeReviewHomePreferenceLevel> HomePreferenceLevels { get; set; }
    }
}
