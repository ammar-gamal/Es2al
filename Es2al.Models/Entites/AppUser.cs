using Microsoft.AspNetCore.Identity;

namespace Es2al.Models.Entites
{
    public class AppUser : IdentityUser<int>
    {
        public string? Bio { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = null!;
        public ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();//all my followers
        public ICollection<UserFollow> Followings { get; set; } = new List<UserFollow>();//all my followings
        public ICollection<Question> SendedQuestions { get; set; } = new List<Question>();
        public ICollection<Question> ReceivedQuestions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<UserReaction> Reactions { get; set; } = new List<UserReaction>();
        public ICollection<UserTag> Tags { get; set; } = new List<UserTag>();
    }
}
