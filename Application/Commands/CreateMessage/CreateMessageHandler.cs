using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Infrastructure;
using Domain;

namespace Application.Commands.CreateMessage
{
    public class CreateMessageHandler: IRequestHandler<CreateMessageCommand, CommandStatus>

    {
        private readonly ApplicationDbContext context;

        public CreateMessageHandler(ApplicationDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            this.context = context;
        }
        public async Task<CommandStatus> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {


            var message = new Message()
            {
               Content = request.Content,
               IntendedFor = request.IntendedFor
            };

            await context.Messages.AddAsync(message, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new CommandStatus();
        }
    }
}
