using Microsoft.EntityFrameworkCore.Storage;

namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task RemoveAsync(int id);
        Task<TEntity?> FindAsync(params object?[]? keyValues);
        IQueryable<TEntity> GetAll();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
