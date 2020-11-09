using System.Collections.Generic;

namespace App.Data.Models
{
    public class DetailCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }

        public List<Detail> Details { get; set; }
    }
}
