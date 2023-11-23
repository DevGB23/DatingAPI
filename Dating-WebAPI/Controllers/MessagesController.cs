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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> CreateMessage([FromBody]
            CreateMessageDTO createMessage)
        {
            string username = User.GetUsername();
            
            if (username == createMessage?.RecipientUsername?.ToLower())
                return BadRequest("You cannot send messages to yourself!");

            AppUser? sender = await _unitOfWork.UserRepository.GetAsync(includeProperties: "", tracked: true, u => u.UserName == username);
            AppUser? recipient = await _unitOfWork.UserRepository.GetAsync(includeProperties: "", tracked: true, u => u.UserName == createMessage.RecipientUsername);

            if (recipient is null) return NotFound();

            Message message = new() 
            {
                Sender = sender,
                SenderUsername = username,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = createMessage.Content
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to create a message");
        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams msgParams)
        {
            msgParams.Username = User.GetUsername();

            PagedList<MessageDTO> messages = await _unitOfWork.MessageRepository.GetMessagesForUser(msgParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, 
                    messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;

            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted || message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            return Ok();
        }
}
