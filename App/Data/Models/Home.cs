using System;
using System.Collections.Generic;

namespace App.Data.Models
{
    public class Home
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<HomeDetail> HomeDetails { get; set; }
    }
}
