using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Helpers;
using ChatApp.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace ChatApp.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddGroup(Group group) => context.Groups.Add(group);

    public void RemoveConnection(Connection connection) => context.Connections.Remove(connection);

    public async Task<Connection?> GetConnection(string? connectionId) =>
       await context.Connections.FindAsync(connectionId);

    public async Task<Group?> GetMessageGroup(string? groupName) =>
       await context.Groups.Include(group => group.Connections)
            .FirstOrDefaultAsync(group => group.Name == groupName);

    public Task<Group?> GetGroupForConnection(string? connectionId) =>
        context.Groups.Include(group => group.Connections)
            .FirstOrDefaultAsync(group => group.Connections.Any(connection => connection.ConnectionId == connectionId));

    public async Task<Message?> GetMessage(int? id) =>
       await context.Messages.FindAsync(id);

    public void AddMessage(Message message) => context.Messages.Add(message);

    public void DeleteMessage(Message message) => context.Messages.Remove(message);

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
           var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

           query = messageParams.Container switch
    {
        "Inbox" => query.Where(x =>
            x.RecipientUsername == messageParams.Username &&
            !x.RecipientDeleted),

        "Outbox" => query.Where(x =>
            x.SenderUsername == messageParams.Username &&
            !x.SenderDeleted),

        _ => query.Where(x =>
            x.RecipientUsername == messageParams.Username &&
            !x.RecipientDeleted &&
            x.DateRead == null)
    };

    Console.WriteLine($"Username: '{messageParams.Username}'");
    Console.WriteLine($"Container: '{messageParams.Container}'");

    var count = await query.CountAsync();

    Console.WriteLine($"Filtered messages: {count}");

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await context.Messages
            .Include(message => message.Sender).ThenInclude(user => user.Photos)
            .Include(message => message.Recipient).ThenInclude(user => user.Photos)
            .Where(message =>
                (message.RecipientUsername == currentUsername && message.SenderUsername == recipientUsername && !message.RecipientDeleted) ||
                (message.RecipientUsername == recipientUsername && message.SenderUsername == currentUsername && !message.SenderDeleted))
            .OrderBy(message => message.MessageSent)
            .ToListAsync();

        foreach (var message in messages.Where(message =>
                     message.RecipientUsername == currentUsername && message.DateRead == null))
        {
            message.DateRead = DateTime.UtcNow;
        }

        return mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    private async Task<PagedList<MessageDto>> GetPagedMessages(IQueryable<Message> messages,
        MessageParams messageParams)
    {
        var totalCount = await messages.CountAsync();
        var page = await messages
            .Include(message => message.Sender).ThenInclude(user => user.Photos)
            .Include(message => message.Recipient).ThenInclude(user => user.Photos)
            .AsNoTracking()
            .Skip((messageParams.PageNumber - 1) * messageParams.PageSize)
            .Take(messageParams.PageSize)
            .ToListAsync();

        return new PagedList<MessageDto>(mapper.Map<IEnumerable<MessageDto>>(page), totalCount,
            messageParams.PageNumber, messageParams.PageSize);
    }
}
