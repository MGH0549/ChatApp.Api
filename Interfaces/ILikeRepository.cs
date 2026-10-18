using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Helpers;

namespace ChatApp.interfaces
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }

}