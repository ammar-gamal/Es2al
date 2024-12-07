using Es2al.Models.Abstractions;

namespace Es2al.Models.Entites
{
    public class Answer : BaseText
    {
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public Question Question{ get; set; } = null!;
        public int QuestionId { get; set; }
        public ICollection<UserReaction> Reactions { get; set; } = new List<UserReaction>(); 
    }
}
