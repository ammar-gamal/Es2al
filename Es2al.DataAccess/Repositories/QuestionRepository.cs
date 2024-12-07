using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;

namespace Es2al.DataAccess.Repositories
{
    public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(AppDbContext context) : base(context) { }
        public async Task <Question>GetQuestionAsync(int questionId, int receiverId)
        {
            return  await _dbSet.FirstAsync(e => e.Id == questionId && e.ReceiverId == receiverId);
        }
        public async Task RemoveAsync(int questionId, int receiverId)
        {
            var question = await _dbSet.FirstAsync(e => e.Id == questionId && e.ReceiverId == receiverId);
            await RemoveAsync(question);

        }
        public IQueryable<Question> GetQuestionsInThread(int threadId)
        {
            return _dbSet.Where(e => e.ThreadId == threadId);
        }

        public IQueryable<Question> GetUserInbox(int userId)
        {
            return _dbSet.Where(e => e.ReceiverId == userId && e.IsAnswered == false);
        }

        public IQueryable<Question> GetFeedQAs(int userId)
        {
            return _dbSet.Where(q => q.IsAnswered == true &&
                                    (q.SenderId == userId ||
                                    q.ReceiverId == userId ||
                                    q.Receiver.Followers.Any(e => e.FollowerId == userId)));
        }

        public IQueryable<Question> GetUserQAs(int userId)
        {
            return _dbSet.Where(q => q.ReceiverId == userId && q.IsAnswered == true);
        }
    }
}
