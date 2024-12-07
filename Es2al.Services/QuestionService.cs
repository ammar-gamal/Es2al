using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Microsoft.EntityFrameworkCore;


namespace Es2al.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionThreadsService _questionThreadsService;
        public QuestionService(IQuestionRepository questionRepository, IQuestionThreadsService questionThreadsService)
        {
            _questionRepository = questionRepository;
            _questionThreadsService = questionThreadsService;
        }
        public async Task CreateQuestionAsync(Question question)
        {
            if (question.ThreadId == 0) //this question is new one and make new thread and add this question to this thread
                await _questionThreadsService.AddQuestionToNewThreadAsync(question);
            else
                await _questionRepository.AddAsync(question);

        }
        public async Task DeleteQuestionAsync(int questionId, int currentUserId)
        {
            var targetQuestion = await _questionRepository.GetQuestionAsync(questionId, currentUserId);
            if (targetQuestion.IsAnswered == false)//not answered questin => just delete it 
                await _questionRepository.RemoveAsync(targetQuestion);
            else
            {//get all questions in this thread and create the tree for this target questions-
                List<Question> deletedQuestions = [targetQuestion];
                var questions = await _questionRepository.GetQuestionsInThread(targetQuestion.ThreadId)
                                                         .GroupBy(e => e.ParentQuestionId ?? 0)
                                                         .ToDictionaryAsync(e => e.Key, e => e.ToHashSet());

                Utilites.MarkChildrenAsDeleted(questions, deletedQuestions, questionId);
                await _questionRepository.RemoveRangeAsync(deletedQuestions);
            }
            if (targetQuestion.ParentQuestionId == null)//delete the thread
                await _questionThreadsService.DeleteThreadAsync(targetQuestion.ThreadId);

        }
        public async Task UpdateQuestionAsync(Question question)
        {
            await _questionRepository.UpdateAsync(question);
        }
        public async Task<QuestionAnswerVM?> GetQuestionAnswerAsync(int questionId, int currentUserId)
        {
            return await _questionRepository.GetAll()
                                            .Where(e => e.Id == questionId)
                                            .Select(Utilites.QuestionAnswerVMProjection(currentUserId))
                                            .FirstOrDefaultAsync();
        }
        public async Task<Question> GetQuestionAsync(int questionId, int receiverId)
        {
            return await _questionRepository.GetQuestionAsync(questionId, receiverId);
        }
        public async Task<Dictionary<int, HashSet<QuestionAnswerVM>>?> GetQuestionsInThreadAsync(int threadId, int userId)
        {
            var query = _questionRepository.GetQuestionsInThread(threadId);
            if (await query.CountAsync() == 0)
                return null;

                var receiverId = await query.Select(e => e.ReceiverId)
                                            .FirstOrDefaultAsync();
            if (receiverId != userId) //not the receiver so you should just get all answered questions only
                query = query.Where(q => q.IsAnswered == true).OrderByDescending(q => q.Date);
            else
                query = query.OrderBy(e => e.IsAnswered).ThenByDescending(q => q.Date);

            var questions = await query.GroupBy(e => e.ParentQuestionId ?? 0, Utilites.QuestionAnswerVMProjection(userId))
                                  .ToDictionaryAsync(e => e.Key, e => e.ToHashSet());

            return questions;
        }
        public async Task<PaginatedList<QuestionAnswerVM>> GetFeedQAsAsync(int userId, int pageIdx, QuestionFilterVM questionFilterVM)
        {
            var feedQA = Utilites.FilterQuestions(_questionRepository.GetFeedQAs(userId), questionFilterVM)
                                 .AsSplitQuery()
                                 .Select(Utilites.QuestionAnswerVMProjection(userId));

            return await PaginatedList<QuestionAnswerVM>.CreateAsync(feedQA, pageIdx, Utilites.ItemsPerPage);
        }
        public async Task<PaginatedList<QuestionVM>> GetUserInboxAsync(int userId, int pageIndex, QuestionFilterVM questionFilterVM)
        {
            var inboxQuestions = Utilites.FilterQuestions(_questionRepository.GetUserInbox(userId), questionFilterVM)
                                         .AsSplitQuery()
                                         .Select(Utilites.InboxQuestionVMProjection(userId));

            return await PaginatedList<QuestionVM>.CreateAsync(inboxQuestions, pageIndex, Utilites.ItemsPerPage);
        }
        public async Task<PaginatedList<QuestionAnswerVM>> GetUserQA(int visiterId, int userId, int pageIndex, QuestionFilterVM questionFilterVM)
        {
            var userQA = Utilites.FilterQuestions(_questionRepository.GetUserQAs(userId), questionFilterVM)
                                 .Select(Utilites.QuestionAnswerVMProjection(visiterId));//visiterId to check if this visiter react to any quesstions or not 

            return await PaginatedList<QuestionAnswerVM>.CreateAsync(userQA, pageIndex, Utilites.ItemsPerPage);
        }

    }
}