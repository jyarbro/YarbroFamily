using System;

namespace App.Data.Models
{
    public class DetailWeight
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int DetailId { get; set; }
        public Detail Detail { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }
    }
}
