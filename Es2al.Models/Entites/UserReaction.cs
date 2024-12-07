using Es2al.Models.Enums;

namespace Es2al.Models.Entites
{
    public class UserReaction
    {
        public int UserId { get; set; }
        public int AnswerId { get; set; }
        public AppUser User { get; set; } = null!;
        public Answer Answer { get; set; } = null!;
        public React React { get; set; }
    }
}
