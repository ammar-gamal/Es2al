using Es2al.DataAccess.Context;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Es2al.Tests.Helpers
{
    public static class EntitesFactory
    {
        public static async Task<AppDbContext> CreateNewContextAsync(Action<AppDbContext>? seedExtra=null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            await context.Tag.AddRangeAsync(GetTags());
            await context.Users.AddRangeAsync(GetAppUsers());
            seedExtra?.Invoke(context);
            await context.SaveChangesAsync();
            return context;
        }
        public static List<Tag> GetTags()
        {
            List<Tag> tags = new();
            for (int i = 1; i <= 20; i++)
            {
                tags.Add(new Tag
                {
                    Id = i,
                    Name = $"Tag #{i}"
                });
            }
            return tags;
        }
        public static List<AppUser> GetAppUsers()
        {
            List<AppUser> users = new();
            for (int i = 1; i <= 20; i++)
            {
                users.Add(new AppUser
                {
                    Id = i,
                    UserName = $"UserName #{i}",
                    PasswordHash = $"PasswordHash #{i}",
                    Email = $"Email #{i}",
                    Bio = $"Bio #{i}",
                    Image = Encoding.ASCII.GetBytes($"Image #{i}"),
                    Tags = new List<UserTag> { new UserTag { TagId = i } },
                });
            }
           
            return users;
        }
        public static List<Question> GetQuestions()
        {
            List<Question> questions = new();
            for (int i = 1; i <= 20; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    SenderId = i,
                    ReceiverId = 21 - i,
                    Text = $"Question #{i}",
                    Tags = new List<QuestionTag>() { new QuestionTag { TagId = i } },
                    ThreadId = i,
                    IsAnonymous = (i % 2 == 0),
                    Date = new DateTime(2025, 1, i, 9, i, i),
                    IsAnswered = (i <= 10)
                });
            }
            return questions;
        }
        public static List<Question> AddQuestion(int questionId,int receiverId)
        {
            var list=GetQuestions();
            list.Add(new Question() { Id = questionId, ReceiverId = receiverId });
            return list;
        }
        public static List<Answer> GetAnswers()
        {
            List<Answer> answers = new();
            for (int i = 1; i <= 10; i++)
            {
                answers.Add(new Answer
                {
                    Id = i,
                    QuestionId = i,
                    UserId = i,
                    Text = $"Answer #{i}",
                    Date = new DateTime(2025, 1, i, 2, i, i)
                });
            }
            return answers;
        }

    }
}
