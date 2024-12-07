using Es2al.Models.Entites;

namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface IAnswerRepository:IBaseRepository<Answer>
    {
        Task<bool> IsAnswerExistAsync(int answerId);
    }
}
