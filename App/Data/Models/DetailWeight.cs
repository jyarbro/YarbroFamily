namespace App.Data.Models
{
    public class DetailWeight
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
