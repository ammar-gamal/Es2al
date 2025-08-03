using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.IServices;
using Es2al.Models.Enums;
using Es2al.Services.CustomException;

namespace Es2al.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IBaseRepository<UserReaction> _reactionRepository;
        private readonly IAnswerRepository _answerRepository;
        public ReactionService(IBaseRepository<UserReaction> reactionRepository, IAnswerRepository answerRepository)
        {
            _reactionRepository = reactionRepository;
            _answerRepository = answerRepository;
        }
        public async Task<int> ReactAsync(int userId, int answerId, string reactStr)
        {

            bool isReact = Enum.TryParse<React>(reactStr, ignoreCase: true, out React react);
            if (isReact && await _answerRepository.IsAnswerExistAsync(answerId))
            {
                var userReact = await _reactionRepository.FindAsync(userId, answerId);//was null => add / was Like ? make it Dislike / was Dislike ? remove 
                if (userReact == null)//no react 
                {
                    await _reactionRepository.AddAsync(new UserReaction
                    {
                        React = react,
                        UserId = userId,
                        AnswerId = answerId
                    });
                    return 1;
                }
                else if (userReact.React == react)//was reacted a react and clicked same react again 
                {
                    await _reactionRepository.RemoveAsync(userReact);
                    return -1;
                }
                else//was like and want to dislike
                {
                    userReact.React = react;
                    await _reactionRepository.UpdateAsync(userReact);
                    return 0;
                }
            }
            else
                throw new AppException();

        }
        public async Task<int> ReactDislikeAsync(int userId, int answerId)
        {
            if (await _answerRepository.IsAnswerExistAsync(answerId))
            {
                var userReact = await _reactionRepository.FindAsync(userId, answerId);
                //was null => add / was Like ? make it Dislike / was Dislike ? remove 
                if (userReact == null)//no react 
                {
                    await _reactionRepository.AddAsync(new UserReaction
                    {
                        React = React.Dislike,
                        UserId = userId,
                        AnswerId = answerId
                    });
                    return 1;
                }
                else if (userReact.React == React.Dislike)//was dislike and clicked again 
                {
                    await _reactionRepository.RemoveAsync(userReact);
                    return -1;
                }
                else//was like and want to dislike
                {
                    userReact.React = React.Dislike;
                    await _reactionRepository.UpdateAsync(userReact);
                    return 0;
                }
            }
            else
                throw new AppException("Error Answer Is Not Exist");

        }
        public async Task<int> ReactLikeAsync(int userId, int answerId)
        {
            if (await _answerRepository.IsAnswerExistAsync(answerId))
            {
                var userReact = await _reactionRepository.FindAsync(userId, answerId);
                //was null => add / was Like ? make it Like / was Like ? remove 
                if (userReact == null)
                {
                    await _reactionRepository.AddAsync(new UserReaction
                    {
                        React = React.Like,
                        UserId = userId,
                        AnswerId = answerId
                    });
                    return 1;
                }
                else if (userReact.React == React.Like)
                {
                    await _reactionRepository.RemoveAsync(userReact);
                    return -1;
                }
                else//was dislike and want to like
                {
                    userReact.React = React.Like;
                    await _reactionRepository.UpdateAsync(userReact);
                    return 0;
                }
            }
            else
                throw new AppException("Error Answer Is Not Exist");

        }

    }
}
