using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.IServices;
using Microsoft.EntityFrameworkCore;


namespace Es2al.Services
{
    public class TagService : ITagService
    {
        private readonly IBaseRepository<Tag> _tagRepostiory;
        public TagService(IBaseRepository<Tag> tagRepository)
        {
            _tagRepostiory = tagRepository;

        }
        public async Task AddTagAsync(Tag tag)
        {
            await _tagRepostiory.AddAsync(tag);
        }

        public async Task<Tag?> GetTagAsync(int id)
        {
            return await _tagRepostiory.FindAsync(id);
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _tagRepostiory.GetAll()
                                       .AsNoTracking()
                                       .ToListAsync();
        }

        public async Task<bool> IsTagNameExistAsync(string tagName)
        {
            return await _tagRepostiory.GetAll()
                                       .AsNoTracking()
                                       .AnyAsync(e => e.Name == tagName);
        }
        public async Task UpdateTagAsync(Tag tag)
        {
            await _tagRepostiory.UpdateAsync(tag);
        }
        public async Task RemoveTagAsync(int id)
        {
            await _tagRepostiory.RemoveAsync(id);
        }

        public async Task<bool> CannotUpdateTagAsync(Tag tag)
        {
            return await _tagRepostiory.GetAll()
                                       .AsNoTracking()
                                       .Where(e => e.Name == tag.Name && e.Id != tag.Id)
                                       .AnyAsync();
        }

        
    }
}
