using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;


namespace Es2al.DataAccess.Repositories
{
    public class QuestionTagRepository : BaseRepository<QuestionTag>, IQuestionTagRepository
    {
        public QuestionTagRepository(AppDbContext context):base(context)
        { }
        public IQueryable<QuestionTag> GetQuestionTags(int questionId)
        {
            return _dbSet.Where(e => e.QuestionId == questionId);
        }
    }
}
