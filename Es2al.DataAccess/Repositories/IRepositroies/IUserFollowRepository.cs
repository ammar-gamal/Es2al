using Es2al.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface IUserFollowRepository:IBaseRepository<UserFollow>
    {
        IQueryable<UserFollow> GetUserFollowers(int userId);
        IQueryable<UserFollow> GetUserFolloweings(int userId);
    }
}
