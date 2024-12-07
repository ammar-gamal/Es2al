

using Es2al.Models.Entites;

namespace Es2al.Services.IServices
{
    public interface IQuestionThreadsService 
    {
        Task AddQuestionToNewThreadAsync(Question question);
        Task DeleteThreadAsync(int threadId);

    }
}
