using ChatApp.Interfaces;
using ChatApp.interfaces;
using AutoMapper;

namespace ChatApp.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public UnitOfWork(DataContext context, IMapper mapper)
    {
        _context = context;
        UserRepository = new UserRepository(context, mapper);
        MessageRepository = new MessageRepository(context, mapper);
        LikeRepository = new LikeRepository(context, mapper);
    }

    public IUserRepository UserRepository { get; }
    public IMessageRepository MessageRepository { get; }
    public ILikeRepository LikeRepository { get; }

    public async Task<bool> Complete() => await _context.SaveChangesAsync() > 0;

    public bool HasChanges() => _context.ChangeTracker.HasChanges();
}
