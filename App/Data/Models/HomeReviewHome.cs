using System;
using System.Collections.Generic;

namespace App.Data.Models {
    public class HomeReviewHome {
        public int Id { get; set; }

        public bool Available { get; set; }
        public bool Finished { get; set; }

        public string Address { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }

        public float Cost { get; set; }
        public float ExtraCost { get; set; }
        public int Space { get; set; }
        public float Bedrooms { get; set; }
        public float Bathrooms { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<HomeReviewLink> Links { get; set; }
        public List<HomeReviewHomeFeature> HomeFeatures { get; set; }
        public List<HomeReviewHomeFeatureChoice> HomeFeatureChoices { get; set; }
    }
}
