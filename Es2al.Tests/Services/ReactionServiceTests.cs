using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Models.Enums;
using Es2al.Services;
using Es2al.Services.CustomException;
using Es2al.Services.IServices;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Tests.Services
{
    public class ReactionServiceTests
    {
        private readonly IReactionService _sut;
        private readonly Mock<IBaseRepository<UserReaction>> _reactionRepository;
        private readonly Mock<IAnswerRepository> _answerRepository;
        public ReactionServiceTests()
        {
            _reactionRepository = new();
            _answerRepository = new();
            _sut = new ReactionService(_reactionRepository.Object, _answerRepository.Object);
        }
        [Theory]
        [InlineData("blabla",false)]
        [InlineData("blabla",true)]
        [InlineData("Like",false)]

        public async Task ReactAsync_InvalidInput_ThrowAppException(string reactStr,bool isAnswerExist)
        {
            //Arrange
            var userId = 1;
            var answerId = 1;
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(isAnswerExist);

            //Act 
            Func<Task> func = async () => await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            await func.Should().ThrowAsync<AppException>();

        }

        [Fact]
        public async Task ReactAsync_InvalidReactStringAndAnswerIsNotExist_ThrowAppException()
        {
            //Arrange
            int userId = 1, answerId = 1;
            string reactStr = "blabla";
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(false);

            //Act
            Func<Task> func = async () => await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            await func.Should().ThrowAsync<AppException>();
        }
        [Fact]
        public async Task ReactAsync_AnswerIsExistAndInvalidReactString_ThrowAppException()
        {
            //Arrange
            int userId = 1, answerId = 1;
            string reactStr = "blabla";
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(true);

            //Act
            Func<Task> func = async () => await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            await func.Should().ThrowAsync<AppException>();
        }
        [Fact]
        public async Task ReactAsync_AnswerIsNotExistAndValidReactString_ThrowAppException()
        {
            //Arrange
            int userId = 1, answerId = 1;
            string reactStr = React.Like.ToString();
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(false);

            //Act
            Func<Task> func = async () => await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            await func.Should().ThrowAsync<AppException>();
        }

        [Fact]
        public async Task ReactAsync_UserIsNotPreviousReactedToAnswer_AddNewUserReaction()
        {
            //Arrange
            int userId = 1, answerId = 1;
            var react = React.Like;
            string reactStr = React.Like.ToString();
            
            var expectedUserReaction = new UserReaction
            {
                React = react,
                UserId = userId,
                AnswerId = answerId
            };
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(true);
            _reactionRepository.Setup(e => e.FindAsync(userId, answerId)).ReturnsAsync((UserReaction?)null);
            UserReaction capturedUserReaction = null;
            _reactionRepository.Setup(e => e.AddAsync(It.IsAny<UserReaction>())).Callback<UserReaction>(q => capturedUserReaction = q); ;

            //Act
            var res = await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            res.Should().Be(1);
            _reactionRepository.Verify(r => r.AddAsync(It.IsAny<UserReaction>()), Times.Once);
            capturedUserReaction.Should().BeEquivalentTo(expectedUserReaction);
        }

        [Fact]
        public async Task ReactAsync_UserIsPreviousReactedAndClickedSameReact_RemoveTheReaction()
        {
            //Arrange
            int userId = 1, answerId = 1;
            var react = React.Like;
            string reactStr = React.Like.ToString();
            var userReact = new UserReaction
            {
                React = react,
                UserId = userId,
                AnswerId = answerId
            };
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(true);
            _reactionRepository.Setup(e => e.FindAsync(userId, answerId)).ReturnsAsync(userReact);
            _reactionRepository.Setup(e => e.RemoveAsync(userReact)).Verifiable();

            //Act
            var res = await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            res.Should().Be(-1);
            _reactionRepository.Verify(e => e.RemoveAsync(userReact), Times.Once);
        }

        [Fact]
        public async Task ReactAsync_UserIsPreviousReactedAndClickedAnotherReact_UpdateTheReaction()
        {
            //Arrange
            int userId = 1, answerId = 1;
            var react = React.Like;
            string reactStr = React.Like.ToString();
            var userReact = new UserReaction
            {
                React = React.Dislike,
                UserId = userId,
                AnswerId = answerId
            };
            _answerRepository.Setup(e => e.IsAnswerExistAsync(answerId)).ReturnsAsync(true);
            _reactionRepository.Setup(e => e.FindAsync(userId, answerId)).ReturnsAsync(userReact);
            _reactionRepository.Setup(e => e.UpdateAsync(userReact)).Verifiable();

            //Act
            var res = await _sut.ReactAsync(userId, answerId, reactStr);

            //Assert
            res.Should().Be(0);
            userReact.React.ToString().Should().Be(reactStr);
            _reactionRepository.Verify(e=>e.UpdateAsync(userReact),Times.Once);
        }
    }
}
