using System.ComponentModel.DataAnnotations;


namespace Es2al.Services.ViewModels
{

    public class NewAnswerVM
    {
        [MinLength(1)]
        public string Text { get; set; }
        public int QuestionId { get; set; }
    }
}