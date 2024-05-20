using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Shared;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDtoPost dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }
            if (dto == null)
            {
                return BadRequest("Message data is null");
            }

            var message = new Message
            {

                Content = dto.Content,
                IntendedFor = dto.IntendedFor,
                CreatedDate = DateTime.Now,
                CreatedBy = user
            };

            try
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]

        [Route("community")]
        public async Task<List<MessageDto>> GetMessagesCommunity()
        {
            var messages = await _context.Messages.Include(m=>m.CreatedBy).Where(c => c.IntendedFor == "Community").ToListAsync();
            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }

        [HttpGet]

        [Route("moderator")]
        public async Task<List<MessageDto>> GetMessagesModerator()
        {
            var messages = await _context.Messages.Where(c => c.IntendedFor == "Moderator").ToListAsync();
            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString()
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }

        [HttpGet]

        [Route("psychologist")]
        public async Task<List<MessageDto>> GetMessagesPsychologist()
        {
            var messages = await _context.Messages.Where(c => c.IntendedFor == "Psychologist").ToListAsync();
            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString()
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<MessageDto> GetMessageById(Guid id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null) throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound); ;
            MessageDto messageDto = new MessageDto
            {
                Id = message.Id.ToString(),
                Content = message.Content,
                IntendedFor = message.IntendedFor,
                CreatedAt = message.CreatedDate,
                UserId = message.CreatedBy.Id.ToString()
            };


            return messageDto;

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMessageById(Guid id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null) throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound); ;
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateMessage(Guid id, [FromBody] MessageDtoPost dto)
        {
            if (dto == null)
            {
                return BadRequest("Message data is null");
            }

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound(); // or return NotFound("Message not found") if you want to include a message
            }

            // Update only the properties that are present in the DTO
            if (dto.Content != null)
            {
                message.Content = dto.Content;
            }
            if (dto.IntendedFor != null)
            {
                message.IntendedFor = dto.IntendedFor;
            }
            if (dto.CreatedAt != default)
            {
                message.CreatedDate = dto.CreatedAt;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
