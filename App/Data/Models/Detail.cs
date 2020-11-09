namespace App.Data.Models
{
    public class Detail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }

        public int CategoryId { get; set; }
        public DetailCategory Category { get; set; }
    }
}
