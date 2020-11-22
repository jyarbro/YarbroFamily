using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Areas.HomeReviews.Models.Input {
    public class Home {
        [HiddenInput]
        public int Id { get; set; }

        [HiddenInput]
        public string Url { get; set; }

        [Display(Name="Still on the market?")]
        public bool Available { get; set; }

        [Display(Name = "Address")]
        [MinLength(6)]
        [MaxLength(256)]
        public string Address { get; set; }

        [Display(Name = "House Number")]
        [MinLength(1)]
        [MaxLength(32)]
        [RegularExpression(@"^([a-zA-Z0-9\.-]+)$", ErrorMessage = "This must be letters, numbers, periods, and dashes.")]
        public string HouseNumber { get; set; }

        [Display(Name = "Street")]
        [MinLength(1)]
        [MaxLength(128)]
        [RegularExpression(@"^([a-zA-Z0-9 \.&'-]+)$", ErrorMessage = "This must be letters, numbers, and certain special characters.")]
        public string StreetName { get; set; }

        [MinLength(1)]
        [MaxLength(64)]
        [RegularExpression(@"^([a-zA-Z0-9 \.&'-]+)$", ErrorMessage = "This must be letters, numbers, and certain special characters.")]
        public string City { get; set; }

        public string State { get; set; }

        [Range(10000, 99999)]
        [Display(Name = "Zip Code")]
        public int Zip { get; set; }

        [Range(1, 10)]
        public float Bedrooms { get; set; }

        [Range(1, 10)]
        public float Bathrooms { get; set; }

        [Range(1, 10000)]
        public float Cost { get; set; }

        [Range(1, 5000)]
        public int Space { get; set; }
    }
}
