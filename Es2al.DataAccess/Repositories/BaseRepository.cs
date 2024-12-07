using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;

namespace Es2al.DataAccess.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }
        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await SaveAsync();
        }
        public async Task RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task RemoveAsync(int id)
        {
            TEntity? entity = await FindAsync(id);
            if (entity is not null)
                await RemoveAsync(entity);
            else
                throw new KeyNotFoundException($"Id {id} is not found");
        }

        public IQueryable<TEntity> GetAll()
        {
           return  _dbSet.AsQueryable();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
     
        public async Task<TEntity?> FindAsync(params object?[]? keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }
        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            await SaveAsync();
        }
        protected async Task SaveAsync() => await _context.SaveChangesAsync();

       
    }
}
