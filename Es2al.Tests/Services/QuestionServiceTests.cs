using Es2al.Services;
using Es2al.Services.IServices;
using Es2al.DataAccess.Repositories.IRepositroies;
using Moq;
using Es2al.Models.Entites;
using MockQueryable;
using FluentAssertions;
using Es2al.Services.ViewModels;
using Es2al.Models.Enums;
using Es2al.Services.ExtensionMethods;
using System.Text;

namespace Es2al.Tests.Services
{
    public class QuestionServiceTests
    {
   

        private readonly IQuestionService _sut;
        private readonly Mock<IQuestionRepository> _questionRepository;
        private readonly Mock<IQuestionThreadsService> _questionThreadService;
        public QuestionServiceTests()
        {
            _questionRepository = new();
            _questionThreadService = new();
            _sut = new QuestionService(_questionRepository.Object, _questionThreadService.Object);
        }

        [Fact]
        public async Task CreateQuestionAsync_QuestionNotBelongToThread_CreateAndAddQuestionToNewThread()
        {
            //Arrange
            Question question = new();

            //Act
            await _sut.CreateQuestionAsync(question);

            //Assert
            _questionThreadService.Verify(s => s.AddQuestionToNewThreadAsync(question), Times.Once);
            _questionRepository.Verify(s => s.AddAsync(It.IsAny<Question>()), Times.Never);
        }
        [Fact]
        public async Task CreateQuestionAsync_QuestionBelongToThread_AddQuestion()
        {
            //Arrange
            Question question = new() { ThreadId = 5 };

            //Act
            await _sut.CreateQuestionAsync(question);

            //Assert
            _questionRepository.Verify(r => r.AddAsync(question), Times.Once);
            _questionThreadService.Verify(s => s.AddQuestionToNewThreadAsync(question), Times.Never);

        }
        [Fact]
        public async Task CreateQuestionAsync_NullQuestion_ThrowsArgumentNullException()
        {
            // Act
            Func<Task> action = async () => await _sut.CreateQuestionAsync(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeleteQuestionAsync_SubQuestionIsNotAnswered_DeleteQuestionOnly()
        {
            //Arrange
            var userId = 10;
            var questionId = 1;
            var threadId = 5;
            var question = new Question { Id = questionId, IsAnswered = false, ThreadId = threadId, ParentQuestionId = 2 };

            _questionRepository.Setup(r => r.GetQuestionAsync(questionId, userId))
                                                  .ReturnsAsync(question);
            //Act
            await _sut.DeleteQuestionAsync(questionId, userId);

            //Assert
            _questionRepository.Verify(r => r.RemoveAsync(question), Times.Once);
            _questionRepository.Verify(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<Question>>()), Times.Never);
            _questionThreadService.Verify(s => s.DeleteThreadAsync(It.IsAny<int>()), Times.Never);

        }
        [Fact]
        public async Task DeleteQuestionAsync_AnsweredQuestionWithNoChildren_DeleteQuestionOnly()
        {
            // Arrange
            var userId = 10;
            var questionId = 1;
            var threadId = 5;
            var targetQuestion = new Question { Id = questionId, IsAnswered = true, ThreadId = threadId, ParentQuestionId = 11 };
            var questionsInThread = new List<Question> { targetQuestion };

            _questionRepository.Setup(r => r.GetQuestionAsync(questionId, userId)).ReturnsAsync(targetQuestion);
            _questionRepository.Setup(r => r.GetQuestionsInThread(threadId)).Returns(questionsInThread.AsQueryable().BuildMock());

            List<Question> captured = null;
            _questionRepository.Setup(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<Question>>()))
                               .Callback<IEnumerable<Question>>(q => captured = q.ToList());

            // Act
            await _sut.DeleteQuestionAsync(questionId, userId);

            // Assert
            captured.Should().BeEquivalentTo(new List<Question> { targetQuestion });
            _questionRepository.Verify(r => r.RemoveAsync(It.IsAny<Question>()), Times.Never);
            _questionThreadService.Verify(s => s.DeleteThreadAsync(It.IsAny<int>()), Times.Never);
        }
        [Fact]
        public async Task DeleteQuestionAsync_QuestionIsAnsweredAndIsPartOfGraph_DeleteQuestionAndHierarchyOfQuestion()
        {
            //Arrange
            var userId = 10;
            var questionId = 1;
            var threadId = 5;
            var targetQuestion = new Question { Id = questionId, IsAnswered = true, ThreadId = threadId, ParentQuestionId = 11 };
            var questionsNotInDeleteHirarchy = new List<Question>()
                                {new() {Id = 11, IsAnswered = true, ParentQuestionId = null, ThreadId = threadId },
                                new() {Id = 12, IsAnswered = true, ParentQuestionId = 11, ThreadId = threadId },
                                new() {Id = 13, IsAnswered = true, ParentQuestionId = 12, ThreadId = threadId }};

            var questionsToDelete = new List<Question>{targetQuestion,
                                   new() { Id = 4, IsAnswered = true, ParentQuestionId = questionId, ThreadId = threadId},
                                   new() { Id = 6, IsAnswered = true, ParentQuestionId = questionId, ThreadId=threadId},
                                   new() { Id = 7, IsAnswered = true, ParentQuestionId = questionId, ThreadId=threadId},
                                   new() { Id = 8, IsAnswered = true, ParentQuestionId = 7, ThreadId=threadId},
                                   new() { Id = 9, IsAnswered = true, ParentQuestionId = 6, ThreadId=threadId},
                                   new() { Id = 10, IsAnswered = true, ParentQuestionId = 6,ThreadId=threadId}};

            var questionsInThread = questionsToDelete.Union(questionsNotInDeleteHirarchy).ToList();
            _questionRepository.Setup(r => r.GetQuestionAsync(questionId, userId))
                               .ReturnsAsync(targetQuestion);
            _questionRepository.Setup(r => r.GetQuestionsInThread(threadId))
                               .Returns(questionsInThread.AsQueryable().BuildMock());

            List<Question> capturedQuestions = new();
            _questionRepository.Setup(r => r.RemoveRangeAsync(It.IsAny<List<Question>>()))
                                    .Callback<IEnumerable<Question>>(e => capturedQuestions = e.ToList());


            //Act
            await _sut.DeleteQuestionAsync(questionId, userId);

            //Assert
            capturedQuestions.Should().BeEquivalentTo(questionsToDelete);
            _questionRepository.Verify(r => r.RemoveAsync(It.IsAny<Question>()), Times.Never);
            _questionThreadService.Verify(s => s.DeleteThreadAsync(It.IsAny<int>()), Times.Never);
        }
        [Fact]
        public async Task DeleteQuestionAsync_QuestionIsRoot_DeleteQuestionAndThread()
        {
            //Arrange
            var userId = 10;
            var questionId = 1;
            var threadId = 5;
            var targetQuestion = new Question { Id = questionId, IsAnswered = true, ThreadId = threadId, ParentQuestionId = null };
            var grandchild = new Question { Id = 3, ParentQuestionId = questionId, ThreadId = threadId };
            var questionsInThread = new List<Question> { targetQuestion, grandchild };

            _questionRepository.Setup(r => r.GetQuestionAsync(questionId, userId)).ReturnsAsync(targetQuestion);
            _questionRepository.Setup(r => r.GetQuestionsInThread(threadId)).Returns(questionsInThread.AsQueryable().BuildMock());
            List<Question> capturedQuestions = null;
            _questionRepository.Setup(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<Question>>()))
                               .Callback<IEnumerable<Question>>(q => capturedQuestions = q.ToList());
            //Act
            await _sut.DeleteQuestionAsync(questionId, userId);

            //Assert
            capturedQuestions.Should().BeEquivalentTo(questionsInThread);
            _questionRepository.Verify(r => r.RemoveAsync(It.IsAny<Question>()), Times.Never);
            _questionThreadService.Verify(s => s.DeleteThreadAsync(threadId), Times.Once);
        }


        [Fact]
        public async Task GetQuestionAnswerAsync_QuestionNotAnswered_ReturnNull()
        {
            //Arrange
            int currentUserId = 6;
            int targetQuestionId = 5;
            var questions = GetQuestionsData();
            _questionRepository.Setup(r => r.GetAll()).Returns(questions.AsQueryable().BuildMock());

            //Act
            var questionAnswer = await _sut.GetQuestionAnswerAsync(targetQuestionId, currentUserId);

            //Assert
            questionAnswer.Should().BeNull();

        }
        [Fact]
        public async Task GetQuestionAnswerAsync_QuestionDoesNotExist_ReturnsNull()
        {
            //Arrange
            int currentUserId = 6;
            int targetQuestionId = 999;
            var questions = GetQuestionsData();

            _questionRepository.Setup(r => r.GetAll()).Returns(questions.AsQueryable().BuildMock());

            //Act
            var questionAnswer = await _sut.GetQuestionAnswerAsync(targetQuestionId, currentUserId);

            //Assert
            questionAnswer.Should().BeNull();

        }
        [Fact]
        public async Task GetQuestionAnswerAsync_QuestionIsAnsweredAndExists_ReturnsQuestionAnswerVM()
        {
            //Arrange
            int currentUserId = 6;
            int targetQuestionId = 1;
            var questions = GetQuestionsData();
            _questionRepository.Setup(r => r.GetAll()).Returns(questions.AsQueryable().BuildMock());

            //Act
            var questionAnswer = await _sut.GetQuestionAnswerAsync(targetQuestionId, currentUserId);

            //Assert
            questionAnswer.Should().BeOfType<QuestionAnswerVM>();
            questionAnswer.Question.QuestionId.Should().Be(targetQuestionId);

        }
        [Fact]
        public async Task GetQuestionAnswerAsync_RepositoryIsEmpty_ReturnNull()
        {
            // Arrange
            _questionRepository.Setup(r => r.GetAll()).Returns(new List<Question>().AsQueryable().BuildMock());

            // Act
            var questionAnswer = await _sut.GetQuestionAnswerAsync(1, 6);

            // Assert
            questionAnswer.Should().BeNull();
        }

        [Fact]
        public async Task GetQuestionsInThreadAsync_ThreadIsNotExist_ReturnNull()
        {
            //Arrange
            var threadId = 999;
            var userId = 3;
            var questions = new List<Question>();
            _questionRepository.Setup(repo => repo.GetQuestionsInThread(threadId)).Returns(questions.AsQueryable().BuildMock());

            //Act
            var res = await _sut.GetQuestionsInThreadAsync(threadId, userId);

            //Assert
            res.Should().BeNull();
        }
        [Fact]
        public async Task GetQuestionsInThreadAsync_NotEmptyThreadAndUserIsNotReceiver_ReturnAnsweredQuestionOrderedDescByDate()
        {
            //Arrange
            var threadId = 1;
            var userId = 999;

            var questions = GetQuestionsData().Where(e => e.ThreadId == threadId);
            _questionRepository.Setup(repo => repo.GetQuestionsInThread(threadId)).Returns(questions.AsQueryable().BuildMock());

            //Act
            var res = await _sut.GetQuestionsInThreadAsync(threadId, userId);

            //Assert
            res.Should().NotBeNull();
            foreach (var items in res.Values)
            {
                items.Should().BeInDescendingOrder(e => e.Question.Date);
                items.Should().AllSatisfy(q => q.Answer.Should().NotBeNull());
            }

        }
        [Fact]
        public async Task GetQuestionsInThreadAsync_NotEmptyThreadAndUserIsReceiver_ReturnAnsweredQuestionOrderedIsAnsweredAsecAndDescByDate()
        {
            //Arrange
            var threadId = 2;
            var userId = 2;
            var questions = GetQuestionsData().Where(e => e.ThreadId == threadId);
            _questionRepository.Setup(repo => repo.GetQuestionsInThread(threadId)).Returns(questions.AsQueryable().BuildMock());

            //Act
            var res = await _sut.GetQuestionsInThreadAsync(threadId, userId);

            //Assert
            res.Should().NotBeNull();
            var exepctedCount = res.Values.SelectMany(c => c).ToList().Count();
            exepctedCount.Should().Be(questions.Count());

        }

        [Fact]
        public void ApplyFilter_NotProvidedFilter_ReturnQuestionsSortedAscendingByDate()
        {
            //Arrange
            QuestionFilterVM? filterVM = null;
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM);

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
        }
        [Fact]
        public void ApplyFilter_OnlyDateFrom_ReturnOnlyQuestionOnOrAfterFromDate()
        {
            var now = DateTime.Now;
            QuestionFilterVM? filterVM = new()
            {
                DateFrom = DateOnly.FromDateTime(now.AddDays(-6)),//6 Days Before Current Date The Result Is Included In Filter
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrAfter(now.Date.AddDays(-6)));
        }
        [Fact]
        public void ApplyFilter_OnlyDateEnd_ReturnOnlyQuestionOnOrBeforeDateEnd()
        {
            var now = DateTime.Now;
            QuestionFilterVM? filterVM = new()
            {
                DateEnd = DateOnly.FromDateTime(now.AddDays(-3))//3 Days Before Current Date The Result Is Included In Filter
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrBefore(now.Date.AddDays(-3)));
        }
        [Fact]
        public void ApplyFilter_SortOrderIsDescAndProvidedDateRange_ReturnQuestionsSortedDescendingByDateWithinGivenDateRange()
        {
            //Arrange
            var now = DateTime.Now;
            QuestionFilterVM? filterVM = new()
            {
                SortOrder = "desc",
                DateFrom = DateOnly.FromDateTime(now.AddDays(-6)),//6 Days Before Current Date The Result Is Included In Filter
                DateEnd = DateOnly.FromDateTime(now.AddDays(-3))//3 Days Before Current Date The Result Is Included In Filter
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInDescendingOrder(q => q.Date);
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrAfter(now.Date.AddDays(-6)));
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrBefore(now.Date.AddDays(-3)));

        }
        [Fact]
        public void ApplyFilter_NotProvidedSortOrderAndProvidedDateRange_ReturnQuestionsSortedAscnedingByDateWithinGivenDateRange()
        {
            //Arrange
            var now = DateTime.Now;
            QuestionFilterVM? filterVM = new()
            {
                DateFrom = DateOnly.FromDateTime(now.AddDays(-6)),//6 Days Before Current Date The Result Is Included In Filter
                DateEnd = DateOnly.FromDateTime(now.AddDays(-3))//3 Days Before Current Date The Result Is Included In Filter
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrAfter(now.Date.AddDays(-6)));
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrBefore(now.Date.AddDays(-3)));

        }
        [Fact]
        public void ApplyFilter_ProvidingSearchKeyWord_ReturnOnlyQuestionsContainTagretKeyword()
        {
            //Arrange
            string keyword = "abc";
            QuestionFilterVM? filterVM = new()
            {
                SearchKeyword = keyword

            };
            var questions = GetQuestionsData();
            var expectedCount = 4;

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().HaveCount(expectedCount);
            res.Should().AllSatisfy(q => q.Text.ToLower().Should().Contain(keyword.ToLower()));

        }
        [Fact]
        public void ApplyFilter_FilteringByTags_ReturnOnlyQuestionsThatContainAtLeastOneOfProvidedTags()
        {
            //Arrange
            var tags = GetTags().Select(t => t.Name).ToList();

            HashSet<string> filterTags = [tags[0], tags[1]];
            QuestionFilterVM? filterVM = new()
            {
                Tags = filterTags

            };
            var questions = GetQuestionsData();
            var expectedCount = 11;


            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().OnlyContain(q => q.Tags.Any(t => filterTags.Contains(t.Tag.Name)));
            res.Should().HaveCount(expectedCount);

        }
        [Fact]
        public void ApplyFilter_EmptyQuestionList_ReturnEmptyList()
        {
            //Arrange
            var tags = GetTags().Select(t => t.Name).ToList();

            HashSet<string> filterTags = [tags[0], tags[1]];
            QuestionFilterVM? filterVM = new()
            {
                Tags = filterTags

            };
            var questions = GetQuestionsData();
            var expectedCount = 11;


            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInAscendingOrder(q => q.Date);
            res.Should().OnlyContain(q => q.Tags.Any(t => filterTags.Contains(t.Tag.Name)));
            res.Should().HaveCount(expectedCount);

        }
        [Fact]
        public void ApplyFilter_CombiningMultipleFilters_ReturnsCorrectlyFilteredQuestions()
        {
            //Arrange
            var now = DateTime.Now;
            string keyword = "abc";
            var tags = GetTags().Select(t => t.Name).ToList();

            QuestionFilterVM? filterVM = new()
            {
                SortOrder = "desc",
                DateFrom = DateOnly.FromDateTime(now.AddDays(-6)),
                DateEnd = DateOnly.FromDateTime(now.AddDays(-3)),
                SearchKeyword = keyword,
                Tags = new HashSet<string> { tags[0] }
            };

            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeInDescendingOrder(q => q.Date);
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrAfter(now.Date.AddDays(-6)));
            res.Should().AllSatisfy(q => q.Date.Date.Should().BeOnOrBefore(now.Date.AddDays(-3)));
            res.Should().AllSatisfy(q => q.Text.ToLower().Should().Contain(keyword.ToLower()));
            res.Should().OnlyContain(q => q.Tags.Any(t => t.Tag.Name == tags[0]));
        }
        [Fact]
        public void ApplyFilter_NoQuestionsMatchCriteria_ReturnsEmptyCollection()
        {
            //Arrange
            QuestionFilterVM? filterVM = new()
            {
                SearchKeyword = "ThisKeywordShouldNotExistInAnyQuestion"
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeEmpty();
        }
        [Fact]
        public void ApplyFilter_NoQuestionsWithTargetTag_ReturnsEmptyCollection()
        {
            //Arrange
            var tags = GetTags().Select(t => t.Name).ToList();
            QuestionFilterVM? filterVM = new()
            {
                Tags = [tags.Last()]
            };
            var questions = GetQuestionsData();

            //Act
            var res = QuestionQueryableExtensions.ApplyFilters(questions.AsQueryable().BuildMock(), filterVM).ToList();

            //Assert
            res.Should().BeEmpty();
        }

        [Fact]
        public void CreateQuestionAnswerVMProjection_NewQuestin_MapToQuestionAnswerVM()
        {
            //Arrange
            int currentUserId = 5;
            Question question = new()
            {
                Id = 10,
                Tags = [],
                Sender = new() { UserName = "#User" },
                Thread = new QuestionThread { Id = 2 }
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Should().BeOfType<QuestionAnswerVM>();

        }

        [Theory]
        [InlineData(1, "User #1", true, 0, "Anonymous")]
        [InlineData(2, "User #2", false, 2, "User #2")]
        public void CreateQuestionAnswerVMProjection_QuestionAnonymityStatus_CorrectSenderDisplay
            (int actualId, string actualUsername, bool isAnonymous, int expectedId, string expectedUsername)
        {
            //Arrange
            int currentUserId = 5;
            Question question = new()
            {
                Id = 1,
                Sender = new() { UserName = actualUsername },
                SenderId = actualId,
                Tags = [],
                ReceiverId = 6,
                IsAnswered = true,
                ParentQuestionId = null,
                ThreadId = 2,
                IsAnonymous = isAnonymous,
                Thread = new QuestionThread { Id = 2 },
                Date = DateTime.Now.AddHours(-5)
            };

            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Question.SenderName.Should().Be(expectedUsername);
            actual.Question.SenderId.Should().Be(expectedId);

        }

        [Theory]
        [InlineData(2, 10, true)]
        [InlineData(1, 0, false)]
        [InlineData(0, 0, false)]
        [InlineData(0, 6, false)]
        public void CreateQuestionAnswerVMProjection_ThreadAnsweredQuestionCount_DeterminesConversationDisplayPermission
            (int answeredQuestionCount, int notAnsweredQuestionCount, bool expected)
        {
            //Arrange
            int currentUserId = 5;
            Question question = new()
            {
                Id = 1,
                Sender = new() { UserName = "#1" },
                SenderId = 2,
                Tags = [],
                ReceiverId = 6,
                IsAnswered = true,
                ParentQuestionId = null,
                ThreadId = 2,
                IsAnonymous = true,
                Thread = new QuestionThread
                {
                    Id = 2,
                    Questions = Enumerable.Range(0, answeredQuestionCount).Select(_ => new Question { IsAnswered = true })
                                .Union(Enumerable.Range(0, notAnsweredQuestionCount).Select(_ => new Question { IsAnswered = false })).ToList()
                },
                Date = DateTime.Now.AddHours(-5)
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Question.DisplayAllConversation.Should().Be(expected);

        }

        [Theory]
        [InlineData(5, 5, true)]
        [InlineData(4, 5, false)]
        public void CreateQuestionAnswerVMProjection_DeletePermission_IsEnabledWhenReceiverIsCurrentUser
            (int currentUserId, int receiverId, bool expected)
        {
            //Arrange
            Question question = new()
            {
                Id = 1,
                Sender = new() { UserName = "#1" },
                SenderId = 2,
                Tags = [],
                ReceiverId = receiverId,
                IsAnswered = true,
                ParentQuestionId = null,
                ThreadId = 2,
                IsAnonymous = true,
                Thread = new QuestionThread { Id = 2, Questions = [new() { IsAnswered = false }, new() { IsAnswered = false }, new() { IsAnswered = true }] },
                Date = DateTime.Now.AddHours(-5)
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.DeletePermission.Should().Be(expected);

        }

        [Theory]
        [InlineData(5, 10, 5, 10)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 5, 0, 5)]
        [InlineData(5, 0, 5, 0)]
        public void CreateQuestionAnswerVMProjection_AnswerReactions_ReturnNumberOfReactionsOnAnswer
              (int likes, int dislikes, int expectedLikes, int expectedDislikes)
        {
            //Arrange
            int currentUserId = 5;
            Question question = new()
            {
                Id = 1,
                Sender = new() { UserName = "#1" },
                SenderId = 2,
                Tags = [],
                ReceiverId = 4,
                IsAnswered = true,
                ParentQuestionId = null,
                ThreadId = 2,
                IsAnonymous = true,
                Thread = new QuestionThread { Id = 2 },
                Date = DateTime.Now.AddHours(-5),
                Answer = new()
                {
                    Id = 1,
                    User = new() { UserName = "#2" },
                    Reactions = Enumerable.Range(0, likes).Select(_ => new UserReaction { React = React.Like })
                                         .Union(
                                         Enumerable.Range(0, dislikes).Select(_ => new UserReaction { React = React.Dislike }))
                                         .ToList()
                }
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Answer.Should().NotBeNull();
            actual.Answer.NumberOfDislikes.Should().Be(expectedDislikes);
            actual.Answer.NumberOfLikes.Should().Be(expectedLikes);

        }

        [Theory]
        [InlineData(React.Dislike, React.Dislike)]
        [InlineData(React.Like, React.Like)]
        [InlineData(null, null)]

        public void CreateQuestionAnswerVMProjection_UserReaction_ReturnUserReactOnAnswer
           (React? react, React? expectedReact)
        {
            //Arrange
            int currentUserId = 20;
            Question question = new()
            {
                Id = 1,
                Sender = new() { UserName = "#1" },
                SenderId = 2,
                Tags = [],
                ReceiverId = 4,
                IsAnswered = true,
                ParentQuestionId = null,
                ThreadId = 2,
                IsAnonymous = true,
                Thread = new QuestionThread { Id = 2 },
                Date = DateTime.Now.AddHours(-5),
                Answer = new()
                {
                    Id = 1,
                    User = new() { UserName = "#2" },
                    Reactions = react is not null ?
                    Enumerable.Range(0, 10)
                              .Select(i => new UserReaction { UserId = i, React = (i % 2 == 0) ? React.Like : React.Dislike })
                              .Append(new UserReaction { UserId = currentUserId, React = (React)react }).ToList()
                                                    : Enumerable.Empty<UserReaction>().ToList()
                }
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Answer.Should().NotBeNull();
            actual.Answer.ReactionByCurrentUser.Should().Be(expectedReact);


        }

        [Fact]
        public void CreateQuestionAnswerVMProjection_NotAnsweredQuestion_AnswerIsNull()
        {
            //Arrange
            int currentUserId = 5;
            Question question = new()
            {
                Id = 10,
                Text = "Test question",
                ThreadId = 20,
                Tags = [],
                Date = DateTime.Now.AddHours(-5),
                Sender = new() { UserName = "TestUser" },
                SenderId = 30,
                IsAnonymous = false,
                ReceiverId = 40,
                Thread = new QuestionThread { Id = 20 }
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Answer.Should().BeNull();

        }

        [Fact]
        public void CreateQuestionAnswerVMProjection_ParentQuestionIdSet_MapsParentQuestion()
        {
            //Arrange
            int currentUserId = 5;
            int parentQuestionId = 6;
            Question question = new()
            {
                Id = 10,
                Text = "Test question",
                ThreadId = 20,
                Tags = [],
                Date = DateTime.Now.AddHours(-5),
                Sender = new() { UserName = "TestUser" },
                SenderId = 30,
                IsAnonymous = false,
                ReceiverId = 40,
                Thread = new QuestionThread { Id = 20 },
                ParentQuestionId = parentQuestionId
            };
            var func = QuestionService.CreateQuestionAnswerVMProjection(currentUserId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.Question.ParentQuestionAnswer.Should().NotBeNull();
            actual.Question.ParentQuestionAnswer.Question.QuestionId.Should().Be(parentQuestionId);

        }

        [Fact]
        public void CreateInboxQuestionVMProjection_NewQuestion_MapToQuestionVM()
        {
            //Arrange
            var question = new Question { Tags = [], Sender = new() { UserName = "#User" } };
            var userId = 4;
            var func = QuestionService.CreateInboxQuestionVMProjection(userId).Compile();

            //Act
            var res = func.Invoke(question);

            //Assert
            res.Should().BeOfType<QuestionVM>();
        }

        [Fact]
        public void CreateInboxQuestionVMProjection_NewQuestionNotHasParent_ParentQuestionAnswerShouldBeNull()
        {
            //Arrange
            var question = new Question { Tags = [], Sender = new() { UserName = "#User" } };
            var userId = 4;
            var func = QuestionService.CreateInboxQuestionVMProjection(userId).Compile();

            //Act
            var actual = func.Invoke(question);

            //Assert
            actual.ParentQuestionAnswer.Should().BeNull();
        }
        [Fact]
        public void CreateInboxQuestionVMProjection_NewQuestionHasParent_ParentQuestionAnswerMapped()
        {
            //Arrange

            AppUser sender = new AppUser { Id = 1, UserName = "JohnDoe" };

            // Creating a sample answer with reactions
            Answer answer = new Answer
            {
                Id = 10,
                Date = DateTime.Now.AddDays(-1),
                UserId = sender.Id,
                User = sender,
                Text = "This is an answer to the parent question.",
                Reactions = [new() { UserId = 2, React = React.Like }, new() { UserId = 3, React = React.Dislike }, new() { UserId = 1, React = React.Like }]
            };

            // Creating the parent question
            Question parentQuestion = new Question
            {
                Id = 100,
                Text = "This is a parent question.",
                Date = DateTime.Now.AddDays(-2),
                IsAnonymous = false,
                Sender = sender,
                SenderId = sender.Id,
                Tags = [new() { Tag = new() { Name = "C#" } }, new() { Tag = new() { Name = "LINQ" } }],
                ThreadId = 200,
                Answer = answer
            };

            // Creating the main question that references the parent question
            Question mainQuestion = new Question
            {
                Id = 101,
                Text = "This is a main question with a parent question.",
                Date = DateTime.Now,
                IsAnonymous = false,
                Sender = sender,
                SenderId = sender.Id,
                Tags = [new() { Tag = new() { Name = "ASP.NET" } }, new() { Tag = new() { Name = "Entity Framework" } }],

                ThreadId = 201,
                ParentQuestionId = parentQuestion.Id,
                ParentQuestion = parentQuestion
            };
            var userId = 4;
            var func = QuestionService.CreateInboxQuestionVMProjection(userId).Compile();

            //Act
            var actual = func.Invoke(mainQuestion);

            //Assert
            actual.ParentQuestionAnswer.Should().NotBeNull();
        }

        private List<Question> GetQuestionsData()
        {
            var tags = GetTags();
            var questions = new List<Question>{
                                   //Thread 1
                                   new() { Id = 1,  IsAnswered=true,  SenderId=1, ReceiverId=1, ParentQuestionId=null, ThreadId=1, Thread=new (), Text="AbCd",  Sender=new (){UserName="#1"},  Tags=[new(){ Tag = tags[0] }, new() { Tag = tags[1] },],  Date=DateTime.Now.AddHours(-20)
                                   ,Answer=new (){Id=1,Reactions=[],User=new()} },

                                   new() { Id = 3, IsAnswered=true, SenderId=3, ReceiverId=1, ParentQuestionId=1, ThreadId=1, Thread=new (), Text="ABCD",  Sender=new (){UserName="#3"},  Tags=[new(){ Tag = tags[1] }, new() { Tag = tags[2] },],  Date=DateTime.Now.AddHours(-10) ,
                                       Answer=new (){Id=1,Reactions=[],User=new()}},

                                   new() { Id = 5, IsAnswered=false, SenderId=4, ReceiverId=1, ParentQuestionId=1, ThreadId=1, Thread=new (), Text="abcd",  Sender=new (){UserName="#5"},  Tags=[new(){ Tag = tags[4] }, new() { Tag = tags[3] },],  Date=DateTime.Now.AddHours(-1)
                                   },

                                   new() { Id = 7, IsAnswered=true, SenderId=6, ReceiverId=1, ParentQuestionId=3, ThreadId=1, Thread=new (), Text="QaBcD",  Sender=new (){UserName="#7"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[4] },],  Date=DateTime.Now.AddDays(-1).AddHours(-20)
                                   ,Answer=new (){Id=1,Reactions=[],User=new()} },

                                   new() { Id = 9, IsAnswered=true, SenderId=6, ReceiverId=1, ParentQuestionId=3, ThreadId=1, Thread=new (), Text="QWER",  Sender=new (){UserName="#7"},  Tags=[new(){ Tag = tags[1] }, new() { Tag = tags[5] },], Date=DateTime.Now.AddDays(-1).AddHours(-10)
                                    ,Answer=new (){Id=1,Reactions=[],User=new()} },

                                   new() { Id = 11, IsAnswered=true, SenderId=6, ReceiverId=1, ParentQuestionId=9, ThreadId=1, Thread=new (), Text="asdfgqw",  Sender=new (){UserName="#7"},  Tags=[new(){ Tag = tags[0] }, new() { Tag = tags[6] },],  Date=DateTime.Now.AddDays(-1).AddHours(-15)
                                   ,Answer=new (){Id=1,Reactions=[],User=new()}},

                                   new() { Id = 13, IsAnswered=false, SenderId=6, ReceiverId=1, ParentQuestionId=9, ThreadId=1, Thread=new (), Text="asdqwevsx",  Sender=new (){UserName="#7"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[4] },],  Date=DateTime.Now.AddDays(-2).AddHours(-10)
                                   },

                                   //Thread 2
                                   new() { Id = 2, IsAnswered=true, SenderId=1, ReceiverId=2, ParentQuestionId=null, ThreadId=2, Thread=new (), Text="Hello",  Sender=new (){UserName="#2"},  Tags=[new(){ Tag = tags[3] }, new() { Tag = tags[5] },],  Date=DateTime.Now.AddDays(-3).AddHours(-20)
                                   ,Answer=new (){Id=1,Reactions=[],User=new()}},

                                   new() { Id = 4, IsAnswered=true, SenderId=4, ReceiverId=2, ParentQuestionId=2, ThreadId=2, Thread=new (), Text="Bye",  Sender=new (){UserName="#4"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[6] },],  Date=DateTime.Now.AddDays(-3).AddHours(-10)
                                    ,Answer=new (){Id=2,Reactions=[],User=new()}},

                                   new() { Id = 6, IsAnswered=true, SenderId=5, ReceiverId=2, ParentQuestionId=4, ThreadId=2, Thread=new (), Text="Wow",  Sender=new (){UserName="#6"},  Tags=[new(){ Tag = tags[1] }, new() { Tag = tags[0] },], Date=DateTime.Now.AddDays(-4).AddHours(-20)
                                   ,Answer=new (){Id=3,Reactions=[],User=new()}},

                                   new() { Id = 8, IsAnswered=true, SenderId=6, ReceiverId=2, ParentQuestionId=6, ThreadId=2, Thread=new (), Text="Amazing",  Sender=new (){UserName="#8"},  Tags=[new(){ Tag = tags[1] }, new() { Tag = tags[5] },],  Date=DateTime.Now.AddDays(-4).AddHours(-10)
                                   ,Answer=new (){Id=4,Reactions=[],User=new()}},

                                   new() { Id = 10, IsAnswered=true, SenderId=6, ReceiverId=2, ParentQuestionId=6, ThreadId=2, Thread=new (), Text="Wonderful",  Sender=new (){UserName="#8"},  Tags=[new(){ Tag = tags[0] }, new() { Tag = tags[6] },], Date=DateTime.Now.AddDays(-5).AddHours(-20)
                                   },

                                   new() { Id = 12, IsAnswered=false, SenderId=6, ReceiverId=2, ParentQuestionId=6, ThreadId=2, Thread=new (), Text="Fine",  Sender=new (){UserName="#8"},  Tags=[new(){ Tag = tags[0] }, new() { Tag = tags[6] },], Date=DateTime.Now.AddDays(-5).AddHours(-10)
                                   },

                                   //Thread 3

                                   new() { Id = 100, IsAnswered=true ,  SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=3 , Thread=new (), Text="TREWQ",  Sender=new (){UserName="#9"} ,  Tags=[new(){ Tag = tags[0] }, new() { Tag = tags[5] },],  Date=DateTime.Now.AddDays(-6).AddHours(-20)
                                   ,Answer=new (){Id=1,Reactions=[],User=new()}},

                                   //Thread 4

                                   new() { Id = 101, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=4,  Thread=new (), Text="QWE",  Sender=new (){UserName="#101"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[3] },],  Date=DateTime.Now.AddDays(-6).AddHours(-10)},

                                   //Thread 5
                                   new() { Id = 102, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=5,  Thread=new (), Text="ANS",  Sender=new (){UserName="#102"},  Tags=[new(){ Tag = tags[4] }, new() { Tag = tags[2] },],  Date=DateTime.Now.AddDays(-6).AddHours(-8)},

                                   //Thread 6
                                   new() { Id = 103, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=6,  Thread=new (), Text="qwe",  Sender=new (){UserName="#103"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[1] },],  Date=DateTime.Now.AddDays(-6).AddHours(-5)},

                                   //Thread 7
                                   new() { Id = 104, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=7,  Thread=new (), Text="dcfsdf",  Sender=new (){UserName="#104"},  Tags=[new(){ Tag = tags[3] }, new() { Tag = tags[4] },],  Date=DateTime.Now.AddDays(-7).AddHours(-2)},
                                     //Thread 8
                                   new() { Id = 105, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=8,  Thread=new (), Text="asd",  Sender=new (){UserName="#105"},  Tags=[new(){ Tag = tags[2] }, new() { Tag = tags[3] },],  Date=DateTime.Now.AddDays(-7).AddHours(-2)},
                                     //Thread 9
                                   new() { Id = 106, IsAnswered=false, SenderId=6, ReceiverId=3, ParentQuestionId=null, ThreadId=9,  Thread=new (), Text="ac",  Sender=new (){UserName="#106"},  Tags=[new(){ Tag = tags[1] }, new() { Tag = tags[4] },],  Date=DateTime.Now.AddDays(-8).AddHours(-2)},




            };
            return questions;
        }
        private List<Tag> GetTags()
        {
            var tags = new List<Tag>
                        {
                            new() { Id = 1, Name = "C#" },
                            new() { Id = 2, Name = "ASP.NET" },
                            new() { Id = 3, Name = "Entity Framework" },
                            new() { Id = 4, Name = "LINQ" },
                            new() { Id = 5, Name = "Database" },
                            new() { Id = 6, Name = "Design Patterns" },
                            new() { Id = 7, Name = "Dependency Injection" },
                            new() { Id = 8, Name = "Unit Testing" },
                        };
            return tags;
        }
    }
}
