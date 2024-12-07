using Es2al.Models.Entites;

namespace Es2al.Services.IServices
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTagsAsync();
        Task<bool> IsTagNameExistAsync(string tagName);
        Task RemoveTagAsync(int id);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task<bool> CannotUpdateTagAsync(Tag tag);
        Task<Tag?> GetTagAsync(int id);
    }
}
