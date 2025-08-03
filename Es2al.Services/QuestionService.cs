using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Microsoft.EntityFrameworkCore;
using Es2al.Services.ExtensionMethods;
using System.Linq.Expressions;
using Es2al.Models.Enums;



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
            ArgumentNullException.ThrowIfNull(question);

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

                MarkChildrenAsDeleted(questions, deletedQuestions, questionId);
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
                                            .Where(e => e.Id == questionId && e.IsAnswered==true)
                                            .Select(CreateQuestionAnswerVMProjection(currentUserId))
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

            var questions = await query.GroupBy(e => e.ParentQuestionId ?? 0, CreateQuestionAnswerVMProjection(userId))
                                       .ToDictionaryAsync(e => e.Key, e => e.ToHashSet());

            return questions;
        }
        public async Task<PaginatedList<QuestionAnswerVM>> GetFeedQAsAsync(int userId, int pageIdx, QuestionFilterVM? questionFilterVM)
        {
            var feedQA = _questionRepository.GetFeedQAs(userId)
                        .ApplyFilters(questionFilterVM)
                        .AsSplitQuery()
                        .Select(CreateQuestionAnswerVMProjection(userId));

            return await PaginatedList<QuestionAnswerVM>.CreateAsync(feedQA, pageIdx, Constants.ItemsPerPage);
        }
        public async Task<PaginatedList<QuestionVM>> GetUserInboxAsync(int userId, int pageIndex, QuestionFilterVM? questionFilterVM)
        {
            var inboxQuestions = _questionRepository.GetUserInbox(userId)
                                  .ApplyFilters(questionFilterVM)
                                  .AsSplitQuery()
                                  .Select(CreateInboxQuestionVMProjection(userId));

            return await PaginatedList<QuestionVM>.CreateAsync(inboxQuestions, pageIndex, Constants.ItemsPerPage);
        }
        public async Task<PaginatedList<QuestionAnswerVM>> GetUserQA(int visiterId, int userId, int pageIndex, QuestionFilterVM? questionFilterVM)
        {
            var userQA = _questionRepository.GetUserQAs(userId)
                          .ApplyFilters(questionFilterVM)
                          .Select(CreateQuestionAnswerVMProjection(visiterId));                         

                        //visiterId to check if this visiter react to any quesstions or not 

            return await PaginatedList<QuestionAnswerVM>.CreateAsync(userQA, pageIndex, Constants.ItemsPerPage);
        }


       private void MarkChildrenAsDeleted(Dictionary<int, HashSet<Question>> graph, List<Question> deletedQuestions, int questionId)
        {
            if (!graph.TryGetValue(questionId, out HashSet<Question>? neighbours))
            {//what is our base-case ? it is a leaf node (leaf node not has any childrens)
                return;
            }
            foreach (var question in neighbours)
            {
                deletedQuestions.Add(question);
                MarkChildrenAsDeleted(graph, deletedQuestions, question.Id);
            }
        }
       public static Expression<Func<Question, QuestionAnswerVM>> CreateQuestionAnswerVMProjection(int currentUserId) => question => new QuestionAnswerVM
        {
            Question = new QuestionVM
            {
                QuestionId = question.Id,
                ThreadId = question.ThreadId,
                Text = question.Text,
                Tags = question.Tags.Select(t => t.Tag.Name).ToList(),
                Date = question.Date,
                SenderName = question.IsAnonymous ? "Anonymous" : question.Sender.UserName!,
                SenderId = question.IsAnonymous ? 0 : question.SenderId,
                IsAnonymous = question.IsAnonymous,
                DisplayAllConversation = question.Thread.Questions.Where(e => e.IsAnswered == true).Count() >= 2,
                ParentQuestionAnswer = !(question.ParentQuestionId.HasValue) ? null : new QuestionAnswerVM { Question = new QuestionVM { QuestionId = question.ParentQuestionId.Value } },//we meed the parent id => DFS
            },
            Answer = question.Answer == null ? null
           : new AnswerVM
           {
               AnswerId = question.Answer.Id,
               Date = question.Answer.Date,
               UserId = question.Answer.UserId,
               Username = question.Answer.User.UserName!,
               Text = question.Answer.Text,
               Image = question.Answer.User.Image,
               NumberOfDislikes = question.Answer.Reactions.Count(e => e.React == React.Dislike),
               NumberOfLikes = question.Answer.Reactions.Count(e => e.React == React.Like),
               ReactionByCurrentUser = question.Answer.Reactions.Where(e => e.UserId == currentUserId)
                                                         .Select(e => (React?)e.React)
                                                         .FirstOrDefault()
           },
            DeletePermission = question.ReceiverId == currentUserId
        };
       public static Expression<Func<Question, QuestionVM>> CreateInboxQuestionVMProjection(int currentUserId) => question => new QuestionVM
        {
            QuestionId = question.Id,
            Text = question.Text,
            Tags = question.Tags.Select(t => t.Tag.Name).ToList(),
            Date = question.Date,
            SenderName = question.IsAnonymous ? "Anonymous" : question.Sender.UserName ?? "Unknown",
            SenderId = question.IsAnonymous ? 0 : question.SenderId,
            IsAnonymous = question.IsAnonymous,
            ThreadId = question.ThreadId,
            ParentQuestionAnswer = question.ParentQuestionId.HasValue ? new QuestionAnswerVM
            {
                Question = new QuestionVM
                {
                    QuestionId = question.ParentQuestion!.Id,
                    ThreadId = question.ParentQuestion.ThreadId,
                    Text = question.ParentQuestion.Text,
                    Tags = question.ParentQuestion.Tags.Select(t => t.Tag.Name).ToList(),
                    Date = question.ParentQuestion.Date,
                    SenderName = question.ParentQuestion.IsAnonymous ? "Anonymous" : question.ParentQuestion.Sender.UserName!,
                    SenderId = question.ParentQuestion.IsAnonymous ? 0 : question.ParentQuestion!.SenderId,
                    IsAnonymous = question.ParentQuestion.IsAnonymous,
                    DisplayAllConversation = true
                },
                Answer = new AnswerVM
                {
                    AnswerId = question.ParentQuestion.Answer.Id,
                    Date = question.ParentQuestion.Answer.Date,
                    UserId = question.ParentQuestion.Answer.UserId,
                    Username = question.ParentQuestion.Answer.User.UserName!,
                    Text = question.ParentQuestion.Answer.Text,
                    Image = question.ParentQuestion.Answer.User.Image,
                    NumberOfDislikes = question.ParentQuestion.Answer.Reactions.Count(e => e.React == React.Dislike),
                    NumberOfLikes = question.ParentQuestion.Answer.Reactions.Count(e => e.React == React.Like),
                    ReactionByCurrentUser = question.ParentQuestion.Answer.Reactions.Where(e => e.UserId == currentUserId)
                                                     .Select(e => (React?)e.React)
                                                     .FirstOrDefault()
                }
            } : null
        };


    }
}