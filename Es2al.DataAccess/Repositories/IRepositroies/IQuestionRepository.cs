using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;


namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface IQuestionRepository:IBaseRepository<Question>
    {
        Task<Question>GetQuestionAsync(int questionId, int receiverId);
        IQueryable<Question> GetQuestionsInThread(int threadId);
        IQueryable<Question> GetUserInbox(int userId);
        IQueryable<Question> GetUserQAs(int userId);
        IQueryable<Question> GetFeedQAs(int userId);
        Task RemoveAsync(int questionId, int receiverId);
    }
}
