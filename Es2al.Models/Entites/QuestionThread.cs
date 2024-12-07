
namespace Es2al.Models.Entites
{
    public class QuestionThread
    {
        public int Id { get; set; }
        public bool IsClosed { get; set; }
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();

    }
}
