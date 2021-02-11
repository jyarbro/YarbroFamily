using System;

namespace App.Data.Models {
    public class HomeReviewHomePreferenceLevel {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public int HomeId { get; set; }
        public HomeReviewHome Home { get; set; }

        public int PreferenceId { get; set; }
        public HomeReviewPreference Preference { get; set; }

        public int PreferenceLevelId { get; set; }
        public HomeReviewPreferenceLevel PreferenceLevel { get; set; }
    }
}
