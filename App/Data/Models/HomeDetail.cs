namespace App.Data.Models
{
    public class HomeDetail
    {
        public int Id { get; set; }
        public bool Value { get; set; }

        public int HomeId { get; set; }
        public Home Home { get; set; }

        public int DetailId { get; set; }
        public Detail Detail { get; set; }
    }
}
