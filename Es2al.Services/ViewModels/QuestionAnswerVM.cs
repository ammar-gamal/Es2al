
namespace Es2al.Services.ViewModels
{
    public class QuestionAnswerVM
    {
        public QuestionVM Question { get; set; }
        public AnswerVM? Answer{ get; set; }
        public bool DeletePermission { get; set; }
    }
}
