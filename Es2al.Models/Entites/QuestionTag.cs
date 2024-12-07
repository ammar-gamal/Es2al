namespace Es2al.Models.Entites
{
    public class QuestionTag
    {
        public int QuestionId { get; set; }
        public int TagId { get; set; }
        public Question Question { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
