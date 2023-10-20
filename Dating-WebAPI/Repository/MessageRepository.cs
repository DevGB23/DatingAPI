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
    public void AddMessage(Message message)
    {
        _context.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        Message messageOut = new();
        Message? message = await _context.Messages.FindAsync(id);
        
        if (message is not null) messageOut = message;
        
        return messageOut;
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        IQueryable<Message> query = _context.Messages
                        .OrderByDescending(m => m.MessageSent)
                        .AsQueryable();

        query = messageParams.Container switch 
        {
            "Inbox" => query = query.Where(u => u.RecipientUsername == messageParams.Username),
            "Outbox" => query = query.Where(u => u.SenderUsername == messageParams.Username),
            _ => query = query.Where(u => u.RecipientUsername == messageParams.Username 
                    && u.DateRead == null) 
        };

        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        List<Message> messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
                m => m.RecipientUsername == currentUserName && 
                    m.SenderUsername == recipientUserName || 
                    m.RecipientUsername == recipientUserName &&
                    m.SenderUsername == currentUserName 
            ).OrderByDescending(m => m.MessageSent).ToListAsync();

        List<Message> unReadMessages = messages.Where(m => m.DateRead is null 
                    && m.RecipientUsername == currentUserName).ToList();

        if (unReadMessages.Any())
        {
            foreach (var message in unReadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        int change = await _context.SaveChangesAsync();

        if (change > 0) return true;
        
        return false;        
    }
}
