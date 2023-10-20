using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;
public class MessagesController : BaseApiController
{
     private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public MessagesController(IMapper mapper, IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> CreateMessage([FromBody]
            CreateMessageDTO createMessage)
        {
            string username = User.GetUsername();
            
            if (username == createMessage?.RecipientUsername?.ToLower())
                return BadRequest("You cannot send messages to yourself!");

            AppUser? sender = await _userRepository.GetAsync(includeProperties: "", tracked: true, u => u.Username == username);
            AppUser? recipient = await _userRepository.GetAsync(includeProperties: "", tracked: true, u => u.Username == createMessage.RecipientUsername);

            if (recipient is null) return NotFound();

            Message message = new() 
            {
                Sender = sender,
                SenderUsername = username,
                Recipient = recipient,
                RecipientUsername = recipient.Username,
                Content = createMessage.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to create a message");
        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams msgParams)
        {
            msgParams.Username = User.GetUsername();

            PagedList<MessageDTO> messages = await _messageRepository.GetMessagesForUser(msgParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, 
                    messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesThread(string username)
        {
            var currentUserName = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }
        // [HttpDelete("{id}")]
        // public async Task<ActionResult> DeleteMessage(int id)
        // {
        //     var username = User.GetUsername();

        //     var message = await _unitOfWork.MessageRepository.GetMessage(id);

        //     if (message.Sender.UserName != username && message.Recipient.UserName != username)
        //         return Unauthorized();

        //     if (message.Sender.UserName == username) message.SenderDeleted = true;

        //     if (message.Recipient.UserName == username) message.RecipientDeleted = true;

        //     if (message.SenderDeleted && message.RecipientDeleted)
        //         _unitOfWork.MessageRepository.DeleteMessage(message);

        //     if (await _unitOfWork.Complete()) return Ok();

        //     return BadRequest("Problem deleting the message");
        // }
}
