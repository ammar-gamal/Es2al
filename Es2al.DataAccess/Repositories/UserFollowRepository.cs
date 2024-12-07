using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.DataAccess.Repositories
{
    public class UserFollowRepository : BaseRepository<UserFollow>, IUserFollowRepository
    {
        public UserFollowRepository(AppDbContext context) : base(context)
        { }

        public IQueryable<UserFollow> GetUserFolloweings(int userId)
        {
            return _dbSet.Where(e => e.FollowerId == userId);
        }

        public IQueryable<UserFollow> GetUserFollowers(int userId)
        {
            return _dbSet.Where(e => e.FollowingId == userId);
        }
    }
}
