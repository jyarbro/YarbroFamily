using System;

namespace App.Data.Models
{
    public class HomeReviewHomeDetail
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public int HomeId { get; set; }
        public HomeReviewHome Home { get; set; }

        public int DetailId { get; set; }
        public HomeReviewDetail Detail { get; set; }
    }
}
