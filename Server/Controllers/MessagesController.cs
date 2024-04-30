using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Shared;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Message data is null");
            }

            var message = new Message
            {
                Content = dto.Content,
                IntendedFor = dto.IntendedFor
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


    }
}
