using GoodOmens.Messages.Commands;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodOmens.Host.CommandHandlers
{
    public class CreateBookHandler : IHandleMessages<CreateBook>
    {
        public Task Handle(CreateBook message)
        {
            Console.Write($"Creating book with title {message.Title}");

            return Task.CompletedTask;
        }
    }
}
