using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands.CreateMessage
{
    public class CreateMessageCommand : IRequest<CommandStatus>
    {
        public string Content { get; set; }
        public string IntendedFor {  get; set; }
    }
}
