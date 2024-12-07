using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.IServices;
using Microsoft.EntityFrameworkCore;


namespace Es2al.Services
{
    public class QuestionTagService : IQuestionTagService
    {
        private readonly IQuestionTagRepository _questionTagRepository;
        public QuestionTagService(IQuestionTagRepository questionTagRepository)
        {
            _questionTagRepository = questionTagRepository;
        }
        public async Task<List<QuestionTag>> GetQuestionTagsAsync(int questionId)
        {
            return await _questionTagRepository.GetQuestionTags(questionId)
                                               .AsNoTracking()
                                               .ToListAsync();
        }
    }
}
