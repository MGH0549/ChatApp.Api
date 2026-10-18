using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Helpers;

namespace ChatApp.interfaces
{
    public interface IUserRepository
    {
        Task Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
    }
}