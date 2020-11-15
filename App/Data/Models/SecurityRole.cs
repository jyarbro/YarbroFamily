using System;
using System.Collections.Generic;

namespace App.Data.Models {
    public class SecurityRole {
        public int Id { get; set; }
        public string Title { get; set; }

        public DateTime Modified { get; set; }
        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

        public List<UserSecurityRole> Users { get; set; }
    }
}
