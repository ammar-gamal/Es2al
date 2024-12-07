using Es2al.Models.Entites;
using Es2al.Services.CustomEventArgs;
using static Es2al.Services.Utilites;

namespace Es2al.Services.IServices
{
    public interface IAnswerService
    {
        public event EventHandlerAsync<QuestionAnswerEventArgs> OnQuestionAnswer;
        public Task SaveAnswerAsync(Answer answer);
        public Task<bool> IsAnswerExistAsync(int answerId);

        
    }
}
