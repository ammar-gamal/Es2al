using Es2al.Models.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Es2al.Models.Entites
{
    public class Question : BaseText
    {
        public bool IsAnonymous { get; set; }
        public bool IsAnswered { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ThreadId { get; set; }
        public int? ParentQuestionId { get; set; }
        public Answer Answer { get; set; } = null!;
        public ICollection<QuestionTag> Tags { get; set; } = null!;
        public QuestionThread Thread { get; set; } = null!;
        public AppUser Sender { get; set; } = null!;
        public AppUser Receiver { get; set; }
        public ICollection<Question> FollowsUpQuestions { get; set; } = null!;
        public Question? ParentQuestion { get; set; }
    }
    public class QuestionEqualityComparer : IEqualityComparer<Question>
    {
        public bool Equals(Question? x, Question? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;

            return x.Id==y.Id;
        }

        public int GetHashCode([DisallowNull] Question obj)
        {
            return obj.Id;
        }
    }
}
