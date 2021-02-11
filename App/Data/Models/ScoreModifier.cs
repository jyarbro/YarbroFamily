using App.Data.Options;
using System;

namespace App.Data.Models {
    public class ScoreModifier {
        public int Id { get; set; }
        public float Baseline { get; set; }
        public float Multiple { get; set; }
        public ScoreModifierType Type { get; set; }
        
        public DateTime Modified { get; set; }

        public string ModifiedById { get; set; }
        public AppUser ModifiedBy { get; set; }

    }
}
