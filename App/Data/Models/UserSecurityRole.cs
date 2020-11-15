namespace App.Data.Models {
    public class UserSecurityRole {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int SecurityRoleId { get; set; }
        public SecurityRole SecurityRole { get; set; }
    }
}
