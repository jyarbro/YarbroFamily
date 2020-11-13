using System;

namespace App.Data.Models {
    public class AppUser {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
