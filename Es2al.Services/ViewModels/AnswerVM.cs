using Es2al.Models.Enums;

namespace Es2al.Services.ViewModels
{
    public class AnswerVM
    {
        public int AnswerId { get; set; }
        public string Text { get; set; }
        public DateTime Date{ get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public byte[]? Image { get; set; } = null;
        public int NumberOfLikes { get; set; }
        public int NumberOfDislikes { get; set; }
        public React? ReactionByCurrentUser{ get; set; }
        public bool IsReactedLike => ReactionByCurrentUser.HasValue == true && React.Like == ReactionByCurrentUser.Value ;
        public bool IsReactedDisLike => ReactionByCurrentUser.HasValue == true && React.Dislike == ReactionByCurrentUser.Value;
        public string GetDate() => Date.ToString("MMM dd, yyyy hh:mm tt");
    }
}
