using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.IServices;

namespace Es2al.Services
{
    public class QuestionThreadsService : IQuestionThreadsService
    {
        private readonly IBaseRepository<QuestionThread> _questionThreads;

        public QuestionThreadsService(IBaseRepository<QuestionThread> questionThreads)
        {
            _questionThreads = questionThreads;
        }

        public async Task AddQuestionToNewThreadAsync(Question question)
        {
            var questionThread = new QuestionThread();
            questionThread.Questions.Add(question);
            await _questionThreads.AddAsync(questionThread);
        }

        public async Task DeleteThreadAsync(int threadId)
        {
            await _questionThreads.RemoveAsync(threadId);
        }

    }
}
