using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Data.Models {
    public class AppUser {
        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<HomeReviewUserWeight> Preferences { get; set; }
    }
}
