using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Helpers;
using ChatApp.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace ChatApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
               .Where(x => x.UserName == username)
               .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
               .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
         .AsQueryable();

            query = query
                .Where(x => x.UserName != userParams.CurrentUsername);

            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                query = query
                    .Where(x => x.Gender == userParams.Gender);
            }

            var members = query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);

            return await PagedList<MemberDto>.CreateAsync(
                members,
                userParams.PageNumber,
                userParams.PageSize);
        }

        public Task<AppUser>? GetUserByIdAsync(int id)
        {
            return _context.Users.Include(user => user.Photos)
                .SingleOrDefaultAsync(user => user.Id == id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {

             return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(user => user.Photos).ToListAsync();
        }

        public async Task Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

    }
}
