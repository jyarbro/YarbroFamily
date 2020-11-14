using System;
using System.Collections.Generic;

namespace App.Data.Models {
    public class Detail {
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
        public DetailCategory Category { get; set; }

        public List<UserPreference> Weights { get; set; }
        public List<HomeDetail> HomeDetails { get; set; }
    }
}
