

using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Es2al.Tests.Repositories
{
    public class QuestionTagRepositoryTests
    {
       

        [Fact]
        public async Task GetQuestionTags_SearchWithQuestionId_ReturnIQueryableOfQuestionTag()
        {
            //Arrange
            var context = await EntitesFactory.CreateNewContextAsync();
            var questionTagRepository = new QuestionTagRepository(context);

            //Act
            var res = questionTagRepository.GetQuestionTags(1);

            //Assert
            res.Should().BeAssignableTo<IQueryable<QuestionTag>>();
        }
        [Fact]
        public async Task GetQuestionTags_SearchWithExistQuestionId_ReturnQuestionTags()
        {

            //Arrange
            var tags = EntitesFactory.GetTags().Take(4);
            var questionId = 2;
            var context = await EntitesFactory.CreateNewContextAsync(context =>
            {
                var question = new Question()
                {
                    Id = questionId,
                    Tags = tags.Select(t => new QuestionTag { TagId = t.Id })
                          .ToList()
                };
                context.Question.Add(question);

            });
            var questionTagRepository = new QuestionTagRepository(context);

            //Act
            var res = questionTagRepository.GetQuestionTags(questionId);

            //Assert
            res.Select(e => e.Tag).Should().Contain(tags);
            res.Select(e => e.Tag).Should().HaveCount(4);
            res.All(q => q.QuestionId == questionId).Should().BeTrue();
        }
    }
}
