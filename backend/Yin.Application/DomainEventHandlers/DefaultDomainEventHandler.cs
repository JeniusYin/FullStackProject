using Yin.Domain.Event;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yin.Application.DomainEventHandlers
{
    public class DefaultDomainEventHandler : INotificationHandler<DefaultDomainEvent>
    {
        public Task Handle(DefaultDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
