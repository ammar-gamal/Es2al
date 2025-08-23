using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services;
using Es2al.Services.CustomException;
using Es2al.Services.Events.CustomEventArgs;
using Es2al.Services.IServices;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using static Es2al.Services.Events.AsyncEventHandlers;

namespace Es2al.Tests.Services
{
    public class AnswerServiceTests
    {
        private readonly IAnswerService _sut;
        private readonly Mock<IAnswerRepository> _answerRepository;
        private readonly Mock<IQuestionService> _questionService;
        private readonly Mock<INotificationService> _notificationService;

        public AnswerServiceTests()
        {
            _answerRepository = new();
            _questionService = new();
            _notificationService = new();
            _sut = new AnswerService(_answerRepository.Object, _questionService.Object,_notificationService.Object);
        }
        [Fact]
        public async Task SaveAnswerAsync_SavingAnswer_TransactionShouldBeginAndCommitAndUpdateQuestionCalled()
        {
            //Arrange
            int questionId = 1, senderId = 1, receiverId = 2, answerId = 1;
            var question = new Question() { SenderId = senderId, ReceiverId = receiverId, Id = questionId };
            var answer = new Answer() { UserId = receiverId, Id = answerId, QuestionId = questionId };

            _questionService.Setup(q => q.GetQuestionAsync(questionId, receiverId))
                            .ReturnsAsync(question)
                            .Verifiable();

            _questionService.Setup(q => q.UpdateQuestionAsync(question))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            var dbContextTransaction = new Mock<IDbContextTransaction>();
            _answerRepository.Setup(a => a.BeginTransactionAsync())
                            .ReturnsAsync(dbContextTransaction.Object);

            //Act
            await _sut.SaveAnswerAsync(answer);

            //Assert
            _answerRepository.Verify();
            _questionService.Verify();
            dbContextTransaction.Verify(e => e.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SaveAnswerAsync_QuestionIsNotAnswered_QuestionShouledMarkedAsAnswered()
        {
            //Arrange
            int questionId = 1, senderId = 1, receiverId = 2, answerId = 1;
            var question = new Question() { SenderId = senderId, ReceiverId = receiverId, Id = questionId };
            var answer = new Answer() { UserId = receiverId, Id = answerId, QuestionId = questionId };

            _questionService.Setup(q => q.GetQuestionAsync(questionId, receiverId))
                            .ReturnsAsync(question);

            var dbContextTransaction = new Mock<IDbContextTransaction>();
            _answerRepository.Setup(a => a.BeginTransactionAsync())
                            .ReturnsAsync(dbContextTransaction.Object);

            //Act
            await _sut.SaveAnswerAsync(answer);

            //Assert
            question.IsAnswered.Should().BeTrue();
            question.Answer.Should().Be(answer);
            
        }
        
        [Fact]
        public async Task SaveAnswerAsync_ExceptionWasThrown_RollbackAndThrowAnAppException()
        {
            //Arrange
            var question = new Question();
            var answer = new Answer();

            _questionService.Setup(q => q.GetQuestionAsync(It.IsAny<int>(), It.IsAny<int>())).Throws<Exception>();

            _questionService.Setup(q => q.UpdateQuestionAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

            var dbContextTransaction = new Mock<IDbContextTransaction>();

            _answerRepository.Setup(a => a.BeginTransactionAsync()).ReturnsAsync(dbContextTransaction.Object);
            dbContextTransaction.Setup(e => e.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            //Act
            Func<Task> func = async () => await _sut.SaveAnswerAsync(answer);

            //Assert
            await func.Should().ThrowAsync<AppException>();
            dbContextTransaction.Verify();
        }

        [Fact]
        public async Task SaveAnswerAsync_WhenQuestionIsAnswered_EnsureThatNotificationIsSended()
        {
            //Arrange
            int questionId = 1, senderId = 1, receiverId = 2;
            var question = new Question() { SenderId = senderId, ReceiverId = receiverId, Id = questionId };
            var answer = new Answer() { UserId = receiverId, Id = 2, QuestionId = questionId };
            var mockDbTransaction = new Mock<IDbContextTransaction>();
            var dbTransaction = mockDbTransaction.Object;

            _answerRepository.Setup(e => e.BeginTransactionAsync()).ReturnsAsync(dbTransaction);
            _questionService.Setup(e => e.UpdateQuestionAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);
            _questionService.Setup(e => e.GetQuestionAsync(questionId, receiverId)).ReturnsAsync(question);
            _notificationService.Setup(e=>e.AnswerNotificationAsync(receiverId,senderId,questionId)).Returns(Task.CompletedTask);
            mockDbTransaction.Setup(e => e.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            //Act
            await _sut.SaveAnswerAsync(answer);

            //Assert
            _notificationService.Verify();
        }
    }
}
