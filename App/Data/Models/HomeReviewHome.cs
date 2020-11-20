﻿using System;
using System.Collections.Generic;

namespace App.Data.Models
{
    public class HomeReviewHome
    {
        public int Id { get; set; }

        public string Address { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<HomeReviewHomeDetail> Details { get; set; }
        public List<HomeReviewLink> Links { get; set; }
    }
}