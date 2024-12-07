
namespace Es2al.Services.ViewModels
{
    public class QuestionVM
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string SenderName { get; set; }
        public int SenderId { get; set; }
        public bool IsAnonymous { get; set; }
        public int? ThreadId { get; set; }
        public bool DisplayAllConversation { get; set; }
        public QuestionAnswerVM? ParentQuestionAnswer { get; set; } = null;
        public DateTime Date { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public string GetDate() => Date.ToString("MMM dd, yyyy hh:mm tt");

    }
}
