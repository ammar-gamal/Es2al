
using Es2al.Models.Entites;

namespace Es2al.Services.IServices
{
    public interface IQuestionTagService
    {
        Task<List<QuestionTag>> GetQuestionTagsAsync(int questionId);
    }
}
