using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
namespace Es2al.DataAccess.Repositories
{
    public class AnswerRepository : BaseRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext context) : base(context)
        {}
        public async Task<bool> IsAnswerExistAsync(int answerId)
        {
            return await _dbSet.AnyAsync(e => e.Id == answerId);
        }
    }
}
