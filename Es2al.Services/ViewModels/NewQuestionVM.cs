using System.ComponentModel.DataAnnotations;

namespace Es2al.Services.ViewModels
{
    public class NewQuestionVM
    {
        [MinLength(2,ErrorMessage ="Minimum Length For Text Is 10")]
        public string Text { get; set; }
        public bool IsAnonymous { get; set; }
        [Required(ErrorMessage = "At Least One Tag Must Be Selected")]
        public HashSet<int> Tags { get; set; }
        public int ReceiverId { get; set; }
    }
}
