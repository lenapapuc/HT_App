using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http;
using Shared;
using Application.Commands.CreateMessage;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MessageController : ControllerBase
    {
        private readonly IMediator mediator;
        public MessageController(IMediator mediator)
        {
            ArgumentNullException.ThrowIfNull(mediator);

            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDto dto)
        {
            var command = new CreateMessageCommand()
            {
                Content = dto.Content,
                IntendedFor = dto.IntendedFor,
            };

            var result = await mediator.Send(command);

            return result.IsSuccessful ? Ok() : BadRequest(result.Error);

        }

    }
}
