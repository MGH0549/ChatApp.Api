using ChatApp.Controllers;
using AutoMapper;
using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Extensions;
using ChatApp.Helpers;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        public MessagesController(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself.");

            var sender = await _unitofwork.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitofwork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _unitofwork.MessageRepository.AddMessage(message);

            if (await _unitofwork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message.");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername()!;
            Console.WriteLine($"USERNAME FROM TOKEN: {messageParams.Username}");
            var messages = await _unitofwork.MessageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _unitofwork.MessageRepository.GetMessage(id);

            if (message == null) return NotFound();

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();
            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) 
            {
                _unitofwork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitofwork.Complete()) return Ok();

            return BadRequest("Failed to delete message.");
        }
    }
    
}