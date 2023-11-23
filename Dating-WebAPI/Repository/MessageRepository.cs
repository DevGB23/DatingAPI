using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Repository;
public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Remove(message); 
        _context.SaveChanges();       
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        Connection? connection = await _context.Connections.FindAsync(connectionId);

        if (connection is null) return new();

        return connection;
    }

    public Task<Group> GetGroupforConnection(string connectionId)
    {
        return _context.Groups
            .Include(c => c.Connections)
            .Where(c => c.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        Message messageOut = new();
        Message? message = await _context.Messages.FindAsync(id);
        
        if (message is not null) messageOut = message;
        
        return messageOut;
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        IQueryable<Message> query = _context.Messages
                        .OrderByDescending(m => m.MessageSent)
                        .AsQueryable();

        query = messageParams.Container switch 
        {
            "Inbox" => query = query.Where(u => u.RecipientUsername == messageParams.Username 
                            && u.RecipientDeleted == false),
            "Outbox" => query = query.Where(u => u.SenderUsername == messageParams.Username 
                            && u.SenderDeleted == false),
            _ => query = query.Where(u => u.RecipientUsername == messageParams.Username 
                    && u.RecipientDeleted == false && u.DateRead == null) 
        };

        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        IQueryable<Message> query = _context.Messages
            .Where(
                m => m.RecipientUsername == currentUserName &&
                    m.RecipientDeleted == false && 
                    m.SenderUsername == recipientUserName || 
                    m.RecipientUsername == recipientUserName &&
                    m.SenderDeleted == false &&
                    m.SenderUsername == currentUserName 
            ).OrderBy(m => m.MessageSent).AsQueryable();

        List<Message> unReadMessages = query.Where(m => m.DateRead == null 
                    && m.RecipientUsername == currentUserName).ToList();

        if (unReadMessages.Any())
        {
            foreach (var message in unReadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return await query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
    
}
