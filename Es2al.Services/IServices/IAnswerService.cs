using Es2al.Models.Entites;
using Es2al.Services.Events.CustomEventArgs;
using static Es2al.Services.Events.AsyncEventHandlers;

namespace Es2al.Services.IServices
{
    public interface IAnswerService
    {
        //public event EventHandlerAsync<QuestionAnswerEventArgs> OnQuestionAnswer;
        public Task SaveAnswerAsync(Answer answer);
        public Task<bool> IsAnswerExistAsync(int answerId);
    }
}
