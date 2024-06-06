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
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        [HttpPost]
        [Route("{messageId}/reply")]
        public async Task<IActionResult> AddReply(Guid messageId, [FromBody] MessageDtoPost dto)
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

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null) throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound); ;

            var reply = new Message
            {

                Content = dto.Content,
                IntendedFor = dto.IntendedFor,
                CreatedDate = DateTime.Now,
                CreatedBy = user
            };

            message.AddReply(reply);

            try
            {
                _context.Messages.Add(reply);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("{messageId}/replies")]
      
        public async Task<List<MessageDto>> GetReplies(Guid messageId)
        {
            var messages = await _context.Messages.Include(m => m.CreatedBy).Include(m=>m.OriginalMessage)
                .Where(c => c.OriginalMessage.Id == messageId)
                .ToListAsync();

            var userIds = messages.Select(m => m.CreatedBy.Id).Distinct().ToList();
            var userRoles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();

            var userRolesDict = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.Select(ur => ur.Name).ToList());

            var messageIds = messages.Select(m => m.Id).ToList();
            var repliesCountDict = await _context.Messages
                .Where(m => messageIds.Contains(m.OriginalMessage.Id))
                .GroupBy(m => m.OriginalMessage.Id)
                .Select(g => new { MessageId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MessageId, g => g.Count);

            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                var roles = userRolesDict.ContainsKey(message.CreatedBy.Id) ? userRolesDict[message.CreatedBy.Id] : new List<string>();
                var repliesCount = repliesCountDict.ContainsKey(message.Id) ? repliesCountDict[message.Id] : 0;

                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name,
                    UserRole = roles,
                    RepliesNumber = repliesCount // Set the replies count
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }

        [HttpGet]

        [Route("community")]
        public async Task<List<MessageDto>> GetMessagesCommunity()
        {
            var messages = await _context.Messages.Include(m => m.CreatedBy)
                .Where(c => c.IntendedFor == "Community")
                .Where(c => c.OriginalMessage == null)
                .ToListAsync();

            var userIds = messages.Select(m => m.CreatedBy.Id).Distinct().ToList();
            var userRoles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();

            var userRolesDict = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.Select(ur => ur.Name).ToList());

            var messageIds = messages.Select(m => m.Id).ToList();
            var repliesCountDict = await _context.Messages
                .Where(m => messageIds.Contains(m.OriginalMessage.Id))
                .GroupBy(m => m.OriginalMessage.Id)
                .Select(g => new { MessageId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MessageId, g => g.Count);

            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                var roles = userRolesDict.ContainsKey(message.CreatedBy.Id) ? userRolesDict[message.CreatedBy.Id] : new List<string>();
                var repliesCount = repliesCountDict.ContainsKey(message.Id) ? repliesCountDict[message.Id] : 0;

                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name,
                    UserRole = roles,
                    RepliesNumber = repliesCount // Set the replies count
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }

        [HttpGet]
        [Route("moderator")]

        public async Task<List<MessageDto>> GetMessagesModerator()
        {
            var messages = await _context.Messages.Include(m => m.CreatedBy)
                .Where(c => c.IntendedFor == "Moderator")
                .Where(c => c.OriginalMessage == null)
                .ToListAsync();

            var userIds = messages.Select(m => m.CreatedBy.Id).Distinct().ToList();
            var userRoles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();

            var userRolesDict = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.Select(ur => ur.Name).ToList());

            var messageIds = messages.Select(m => m.Id).ToList();
            var repliesCountDict = await _context.Messages
                .Where(m => messageIds.Contains(m.OriginalMessage.Id))
                .GroupBy(m => m.OriginalMessage.Id)
                .Select(g => new { MessageId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MessageId, g => g.Count);

            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                var roles = userRolesDict.ContainsKey(message.CreatedBy.Id) ? userRolesDict[message.CreatedBy.Id] : new List<string>();
                var repliesCount = repliesCountDict.ContainsKey(message.Id) ? repliesCountDict[message.Id] : 0;

                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name,
                    UserRole = roles,
                    RepliesNumber = repliesCount // Set the replies count
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }
        [HttpGet]

        [Route("psychologist")]

        public async Task<List<MessageDto>> GetMessagesPsychologist()
        {
            var messages = await _context.Messages.Include(m => m.CreatedBy)
                .Where(c => c.IntendedFor == "Psychologist")
                .Where(c => c.OriginalMessage == null)
                .ToListAsync();

            var userIds = messages.Select(m => m.CreatedBy.Id).Distinct().ToList();
            var userRoles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();

            var userRolesDict = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.Select(ur => ur.Name).ToList());

            var messageIds = messages.Select(m => m.Id).ToList();
            var repliesCountDict = await _context.Messages
                .Where(m => messageIds.Contains(m.OriginalMessage.Id))
                .GroupBy(m => m.OriginalMessage.Id)
                .Select(g => new { MessageId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MessageId, g => g.Count);

            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                var roles = userRolesDict.ContainsKey(message.CreatedBy.Id) ? userRolesDict[message.CreatedBy.Id] : new List<string>();
                var repliesCount = repliesCountDict.ContainsKey(message.Id) ? repliesCountDict[message.Id] : 0;

                MessageDto messageDto = new MessageDto
                {
                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name,
                    UserRole = roles,
                    RepliesNumber = repliesCount // Set the replies count
                };

                messageList.Add(messageDto);
            }

            return messageList;
        }


            [HttpGet]

        [Route("user/{userId}")]
        public async Task<List<MessageDto>> GetMessagesByUserId(Guid userId)
        {
            var messages = await _context.Messages.Include(m => m.CreatedBy).Where(c => c.CreatedBy.Id == userId).Where(c => c.OriginalMessage == null).ToListAsync();
            var userIds = messages.Select(m => m.CreatedBy.Id).Distinct().ToList();
            var userRoles = await _context.UserRoles
        .Where(ur => userIds.Contains(ur.UserId))
        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
        .ToListAsync();
            var userRolesDict = userRoles
        .GroupBy(ur => ur.UserId)
        .ToDictionary(g => g.Key, g => g.Select(ur => ur.Name).ToList());
            List<MessageDto> messageList = new List<MessageDto>();
            foreach (var message in messages)
            {
                var roles = userRolesDict.ContainsKey(message.CreatedBy.Id) ? userRolesDict[message.CreatedBy.Id] : new List<string>();

                MessageDto messageDto = new MessageDto
                {

                    Id = message.Id.ToString(),
                    Content = message.Content,
                    IntendedFor = message.IntendedFor,
                    CreatedAt = message.CreatedDate,
                    UserId = message.CreatedBy.Id.ToString(),
                    Name = message.CreatedBy.Name,
                    UserRole = roles
                };

                messageList.Add(messageDto);
            }


            return messageList;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<MessageDto> GetMessageById(Guid id)
        {
            var message = await _context.Messages
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
            }

            var userId = message.CreatedBy.Id;
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .ToListAsync();

            var repliesCount = await _context.Messages
                .CountAsync(m => m.OriginalMessage.Id == id);

            MessageDto messageDto = new MessageDto
            {
                Id = message.Id.ToString(),
                Content = message.Content,
                IntendedFor = message.IntendedFor,
                CreatedAt = message.CreatedDate,
                UserId = message.CreatedBy.Id.ToString(),
                Name = message.CreatedBy.Name,
                UserRole = userRoles,
                RepliesNumber = repliesCount // Set the replies count
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
