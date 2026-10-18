using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Helpers;

namespace ChatApp.interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection?> GetConnection(string? connectionId);
        Task<Group?> GetMessageGroup(string? groupName);
        Task<Group?> GetGroupForConnection(string? connectionId);
        Task<Message?> GetMessage(int? id);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
    }
}