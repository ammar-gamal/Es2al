using Es2al.Models.Entites;
using Es2al.Models.Enums;
using Es2al.Services.ViewModels;using System.Linq.Expressions;

namespace Es2al.Services
{
    public class Utilites
    {
        public delegate Task EventHandlerAsync(object? sender, EventArgs e);
        public delegate Task EventHandlerAsync<TEventArgs>(object? sender, TEventArgs e) where TEventArgs:EventArgs;
        public static int ItemsPerPage => 5;

        public static Expression<Func<Question, QuestionVM>> InboxQuestionVMProjection(int currentUserId) => question => new QuestionVM
        {
            QuestionId = question.Id,
            Text = question.Text,
            Tags = question.Tags.Select(t => t.Tag.Name).ToList(),
            Date = question.Date,
            SenderName = question.IsAnonymous ? "Anonymous" : question.Sender.UserName ?? "Unknown",
            SenderId = question.IsAnonymous ? 0 : question.SenderId,
            IsAnonymous = question.IsAnonymous,
            ThreadId = question.ThreadId,
            ParentQuestionAnswer = question.ParentQuestionId.HasValue ? new QuestionAnswerVM
            {
                Question = new QuestionVM
                {
                    QuestionId = question.ParentQuestion!.Id,
                    ThreadId = question.ParentQuestion.ThreadId,
                    Text = question.ParentQuestion.Text,
                    Tags = question.ParentQuestion.Tags.Select(t => t.Tag.Name).ToList(),
                    Date = question.ParentQuestion.Date,
                    SenderName = question.ParentQuestion.IsAnonymous ? "Anonymous" : question.ParentQuestion.Sender.UserName!,
                    SenderId = question.ParentQuestion.IsAnonymous ? 0 : question.ParentQuestion!.SenderId,
                    IsAnonymous = question.ParentQuestion.IsAnonymous,
                    DisplayAllConversation = true
                },
                Answer = new AnswerVM
                {
                    AnswerId = question.ParentQuestion.Answer.Id,
                    Date = question.ParentQuestion.Answer.Date,
                    UserId = question.ParentQuestion.Answer.UserId,
                    Username = question.ParentQuestion.Answer.User.UserName!,
                    Text = question.ParentQuestion.Answer.Text,
                    Image = question.ParentQuestion.Answer.User.Image,
                    NumberOfDislikes = question.ParentQuestion.Answer.Reactions.Count(e => e.React == React.Dislike),
                    NumberOfLikes = question.ParentQuestion.Answer.Reactions.Count(e => e.React == React.Like),
                    ReactionByCurrentUser = question.ParentQuestion.Answer.Reactions.Where(e => e.UserId == currentUserId)
                                                      .Select(e => (React?)e.React)
                                                      .FirstOrDefault()
                }
            } : null
        };
        public static Expression<Func<Question, QuestionAnswerVM>> QuestionAnswerVMProjection(int currentUserId) => (question) => new QuestionAnswerVM
        {
            Question = new QuestionVM
            {
                QuestionId = question.Id,
                ThreadId = question.ThreadId,
                Text = question.Text,
                Tags = question.Tags.Select(t => t.Tag.Name).ToList(),
                Date = question.Date,
                SenderName = question.IsAnonymous ? "Anonymous" : question.Sender.UserName!,
                SenderId = question.IsAnonymous ? 0 : question.SenderId,
                IsAnonymous = question.IsAnonymous,
                DisplayAllConversation = question.Thread.Questions.Where(e => e.IsAnswered == true).Count() >= 2,
                ParentQuestionAnswer = !(question.ParentQuestionId.HasValue) ? null : new QuestionAnswerVM { Question = new QuestionVM { QuestionId = question.ParentQuestionId.Value } },//we meed the parent id => DFS
            },
            Answer = question.Answer == null ? null
             : new AnswerVM
             {
                 AnswerId = question.Answer.Id,
                 Date = question.Answer.Date,
                 UserId = question.Answer.UserId,
                 Username = question.Answer.User.UserName!,
                 Text = question.Answer.Text,
                 Image = question.Answer.User.Image,
                 NumberOfDislikes = question.Answer.Reactions.Count(e => e.React == React.Dislike),
                 NumberOfLikes = question.Answer.Reactions.Count(e => e.React == React.Like),
                 ReactionByCurrentUser = question.Answer.Reactions.Where(e => e.UserId == currentUserId)
                                                           .Select(e => (React?)e.React)
                                                           .FirstOrDefault()
             },
            DeletePermission=question.ReceiverId==currentUserId
        };
        public static IQueryable<Question> FilterQuestions(IQueryable<Question> questions, QuestionFilterVM questionFilterVM)
        {
            if (questionFilterVM != null)
            {

                if (questionFilterVM.SortOrder == "desc")
                    questions = questions.OrderByDescending(e => e.Date);
                else
                    questions = questions.OrderBy(e => e.Date);

                if (questionFilterVM.DateFrom.HasValue)
                {
                    questions = questions.Where(e => e.Date >= questionFilterVM.DateTimeFrom);
                }

                if (questionFilterVM.DateEnd.HasValue)
                {
                    questions = questions.Where(e => e.Date <= questionFilterVM.DateTimeEnd);
                }

                if (!string.IsNullOrWhiteSpace(questionFilterVM.SearchKeyword))
                {
                    questions = questions.Where(e =>
                        e.Text.ToLower().Contains(questionFilterVM.SearchKeyword)
                    );
                }

                if (questionFilterVM.Tags != null && questionFilterVM.Tags.Any())
                {
                    questions = questions.Where(q =>
                        q.Tags.Any(tag => questionFilterVM.Tags.Contains(tag.Tag.Name))
                    );
                }
            }
            else
                questions = questions.OrderBy(e => e.Date);

            return questions;
        }
        public static void MarkChildrenAsDeleted(Dictionary<int, HashSet<Question>> graph, List<Question> deletedQuestions, int questionId)
        {
            if (!graph.TryGetValue(questionId, out HashSet<Question>? neighbours))
            {//what is our base-case ? it is a leaf node (leaf node not has any childrens)
                return;
            }
            foreach (var question in neighbours)
            {
                deletedQuestions.Add(question);
                MarkChildrenAsDeleted(graph, deletedQuestions, question.Id);
            }
        }

    }
}
