namespace Es2al.Services.ViewModels
{
    public class UserProfileVM
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Bio { get; set; } = null!;
        public byte[]? Image { get; set; } = null!;
        public List<string>? Tags { get; set; } = null!;
        public FollowerAndFollowingVM FollowerAndFollowingVM { get; set; } = null!;
        public int AnsweredQuestionsCount { get; set; }
    }
}
