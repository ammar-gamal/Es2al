namespace Es2al.Models.Entites
{
    public class UserTag
    {
        public int UserId { get; set; }
        public int TagId { get; set; }
        public AppUser User { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
