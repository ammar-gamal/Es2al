using Es2al.Models.Entites;


namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface IQuestionTagRepository:IBaseRepository<QuestionTag>
    {
        IQueryable<QuestionTag> GetQuestionTags(int questionId);
    }
}
