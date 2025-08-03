using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.Paging;


namespace Es2al.Services.IServices
{
    public interface IQuestionService
    {
        Task CreateQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int questionId, int currentUserId);
        Task<QuestionAnswerVM?> GetQuestionAnswerAsync(int questionId, int currentUserId);
        Task<Question> GetQuestionAsync(int questionId, int receiverId);
        Task<Dictionary<int, HashSet<QuestionAnswerVM>>?> GetQuestionsInThreadAsync(int threadId, int userId);
        Task<PaginatedList<QuestionVM>> GetUserInboxAsync(int userId, int pageIndex, QuestionFilterVM? questionFilterVM);
        Task<PaginatedList<QuestionAnswerVM>> GetFeedQAsAsync(int userId, int pageIndex, QuestionFilterVM? questionFilterVM);
        Task<PaginatedList<QuestionAnswerVM>> GetUserQA(int visiterId,int userId, int pageIndex, QuestionFilterVM? questionFilterVM);

    }
}
