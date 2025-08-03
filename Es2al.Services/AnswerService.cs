using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.CustomException;
using Es2al.Services.Events.CustomEventArgs;
using Es2al.Services.IServices;
using static Es2al.Services.Events.AsyncEventHandlers;


namespace Es2al.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionService _questionService;
        public event EventHandlerAsync<QuestionAnswerEventArgs> OnQuestionAnswer;
        public AnswerService(IAnswerRepository answerRepository, IQuestionService questionService)
        {
            _answerRepository = answerRepository;
            _questionService = questionService;
        }


        public async Task<bool> IsAnswerExistAsync(int answerId)
        {
            return await _answerRepository.IsAnswerExistAsync(answerId);
        }

        public async Task SaveAnswerAsync(Answer answer)
        {
            using (var transaction = await _answerRepository.BeginTransactionAsync())
            {
                try
                {
                    var question = await _questionService.GetQuestionAsync(answer.QuestionId, answer.UserId);

                    if (!question.IsAnswered)
                        question.IsAnswered = true;

                    question.Answer = answer;
                    await _questionService.UpdateQuestionAsync(question);
                    if (OnQuestionAnswer != null)
                    {
                        await OnQuestionAnswer.Invoke(this, new QuestionAnswerEventArgs 
                                                        { UserId=question.SenderId,
                                                          QuestionId=question.Id,
                                                          ReceiverId=question.ReceiverId});
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new AppException(exception.Message);
                }
            }
        }
    }
}
