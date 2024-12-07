using Es2al.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Services.IServices
{
    public interface IReactionService
    {
        Task<int> ReactLikeAsync(int userId, int answerId);
        Task<int> ReactAsync(int userId, int answerId, string react);
        Task<int> ReactDislikeAsync(int userId, int answerId);
    }
}
