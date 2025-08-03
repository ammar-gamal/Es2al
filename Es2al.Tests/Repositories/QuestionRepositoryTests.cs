using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories;
using Es2al.Models.Entites;
using Es2al.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Es2al.Tests.Repositories
{
    public class QuestionRepositoryTests
    {
       

        [Theory]
        [InlineData(30, 25)]
        [InlineData(32, 22)]
        [InlineData(33, 23)]
        public async Task GetQuestionAsync_PersistedQuestionIdAndReceiverId_ReturnFoundQuestion(int questionId, int receiverId)
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddAsync(new Question() { Id = questionId, ReceiverId = receiverId });
            });
            var repository = new QuestionRepository(context);

            //Act            
            var question = await repository.GetQuestionAsync(questionId, receiverId);

            //Assert
            question.Should().NotBeNull();
            question.ReceiverId.Should().Be(receiverId);
            question.Id.Should().Be(questionId);
        }
        [Theory]
        [InlineData(99999, 15)]
        public async Task GetQuestionAsync_NotPersistedQuestionIdAndReceiverId_KeyNotFoundException(int questionId, int receiverId)
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync();
            var repository = new QuestionRepository(context);

            //Act            
            Func<Task> func = async () => await repository.GetQuestionAsync(questionId, receiverId);

            //Assert
            await func.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Theory]
        [InlineData(52, 32)]
        public async Task RemoveAsync_PersistedQuestionIdAndReceivedId_RemoveQuestion(int questionId, int receiverId)
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddAsync(new Question() { Id = questionId, ReceiverId = receiverId });
            });
            var repository = new QuestionRepository(context);

            //Act            
            await repository.RemoveAsync(questionId, receiverId);
            Func<Task> func = async () => await repository.GetQuestionAsync(questionId, receiverId);

            //Assert
            await func.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetQuestionsInThread_PersistedThreadId_ReturnThreadQuestions()
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.AddRangeAsync(EntitesFactory.GetQuestions());
            });
          
            var repository = new QuestionRepository(context);

            //Act            
            var questions = await repository.GetQuestionsInThread(3).ToListAsync();

            //Assert
            questions.Should().OnlyContain(q => q.ThreadId == 3);
            questions.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetQuestionsInThread_ReturnIQueryable()
        {
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.AddRangeAsync(EntitesFactory.GetQuestions());
            });
            var repository = new QuestionRepository(context);

            // Act
            var result = repository.GetQuestionsInThread(1);

            // Assert
            result.Should().BeAssignableTo<IQueryable<Question>>();
        }

        [Theory]
        [InlineData(5)]
        public async Task GetUserInbox_PersistedUserId_ReturnInboxQuestions(int userId)
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync();
            var repository = new QuestionRepository(context);

            //Act            
            var questions = await repository.GetUserInbox(userId).ToListAsync();

            //Assert
            questions.Should().OnlyContain(q => q.ReceiverId == userId && q.IsAnswered == false);
        }

        [Fact]
        public async Task GetUserInbox_ShouldReturnIQueryable()
        {
            // Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
               await ctx.AddRangeAsync(EntitesFactory.GetQuestions());
            });
            var repository = new QuestionRepository(context);

            // Act
            var result = repository.GetUserInbox(1);

            // Assert
            result.Should().BeAssignableTo<IQueryable<Question>>();
        }

        [Fact]
        public async Task GetFeedQAs_UserIsSender_ReturnsOnlySentAnsweredQuestions()
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
               await ctx.AddRangeAsync([new Question { Id = 1, IsAnswered = true, SenderId = 1, ReceiverId = 2 },
                                        new Question { Id = 2, IsAnswered = true, SenderId = 1, ReceiverId = 3 },
                                        new Question { Id = 3, IsAnswered = true, SenderId = 2, ReceiverId = 4 }]);
            });
            
         
            var repository = new QuestionRepository(context);

            //Act            
            var questions = await repository.GetFeedQAs(1).ToListAsync();

            //Assert
            questions.Should().NotBeEmpty();
            questions.Should().OnlyContain(q => q.SenderId == 1);
        }

        [Fact]
        public async Task GetFeedQAs_UserIsReceiver_ReturnsOnlyReceivedAnsweredQuestions()
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.AddRangeAsync([new Question {Id = 1, IsAnswered = true, SenderId = 1, ReceiverId = 2  },
                                        new Question{Id = 2, IsAnswered = true, SenderId = 1, ReceiverId = 3 },
                                        new Question {Id = 3, IsAnswered = true, SenderId = 2, ReceiverId = 3  }]);
            });

            var repository = new QuestionRepository(context);

            //Act            
            var questions = await repository.GetFeedQAs(3).ToListAsync();

            //Assert
            questions.Should().NotBeEmpty();
            questions.Should().OnlyContain(q => q.ReceiverId == 3);
            questions.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetFeedQAs_UserIsFollower_ReturnsOnlyFollowingAnsweredQuestions()
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Users.AddAsync(new AppUser { Id = 21, Followings = [new UserFollow { FollowerId = 21, FollowingId = 2 }] });
                await ctx.Question.AddRangeAsync([new Question { Id = 1, IsAnswered = true, SenderId = 1, ReceiverId = 2  },
                                                  new Question { Id = 2, IsAnswered = true, SenderId = 1, ReceiverId = 3},
                                                  new Question { Id = 3, IsAnswered = true, SenderId = 2, ReceiverId = 3 }]);
            });

            var questionRepository = new QuestionRepository(context);

            //Act            
            var questions = questionRepository.GetFeedQAs(21);
            var test = questions.ToList();
            //Assert
            test.Should().NotBeEmpty();
            test.Should().OnlyContain(q => q.ReceiverId == 2);
            test.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetFeedQAs_NoReleventQuestions_ReturnEmptyList()
        {
            //Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddRangeAsync([new Question {Id = 1, IsAnswered = true, SenderId = 1, ReceiverId = 2 },
                                                  new Question { Id = 2, IsAnswered = true, SenderId = 1, ReceiverId = 3},
                                                  new Question { Id = 3, IsAnswered = true, SenderId = 2, ReceiverId = 3}]);
            });
            var questionRepository = new QuestionRepository(context);

            //Act            
            var questions = await questionRepository.GetFeedQAs(4).ToListAsync();

            //Assert
            questions.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFeedQAs_ShouldReturnIQueryable()
        {
            // Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddRangeAsync(EntitesFactory.GetQuestions());

            });
            var repository = new QuestionRepository(context);

            // Act
            var result = repository.GetFeedQAs(1);

            // Assert
            result.Should().BeAssignableTo<IQueryable<Question>>();
        }

        [Fact]
        public async Task GetUserQAs_NoAnsweredQuestion_ReturnEmptyList()
        {
            //Arrange

            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddRangeAsync([new Question { Id = 1, IsAnswered = false, SenderId = 1, ReceiverId = 2},
                                                  new Question { Id = 2, IsAnswered = false, SenderId = 1, ReceiverId = 3 },
                                                  new Question { Id = 3, IsAnswered = false, SenderId = 2, ReceiverId = 3 }]);
            });
            var questionRepository = new QuestionRepository(context);

            //Act            
            var questions = await questionRepository.GetUserQAs(3).ToListAsync();

            //Assert
            questions.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserQAs_AnsweredQuestion_GetUserQuestionsAnswers()
        {
            //Arrange

            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddRangeAsync([new Question { Id = 1, IsAnswered = true, SenderId = 1, ReceiverId = 2},
                                                  new Question { Id = 2, IsAnswered = true, SenderId = 1, ReceiverId = 3},
                                                  new Question { Id = 3, IsAnswered = true, SenderId = 2, ReceiverId = 3},
                                                  new Question { Id = 4, IsAnswered = false, SenderId = 2, ReceiverId = 3  }]);
            });
            var questionRepository = new QuestionRepository(context);
            //Act            
            var questions = await questionRepository.GetUserQAs(3).ToListAsync();

            //Assert
            questions.Should().NotBeEmpty();
            questions.Should().HaveCount(2);
            questions.Should().OnlyContain(e => e.IsAnswered == true);
        }

        [Fact]
        public async void GetUserQAs_ShouldReturnIQueryable()
        {
            // Arrange
            await using var context = await EntitesFactory.CreateNewContextAsync(async ctx =>
            {
                await ctx.Question.AddRangeAsync(EntitesFactory.GetQuestions());
            });
        
            var repository = new QuestionRepository(context);

            // Act
            var result = repository.GetUserQAs(1);

            // Assert
            result.Should().BeAssignableTo<IQueryable<Question>>();
        }


    }
}
