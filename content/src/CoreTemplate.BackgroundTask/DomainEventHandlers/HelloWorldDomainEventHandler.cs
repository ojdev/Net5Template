using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CoreTemplate.Domain.Events;

namespace CoreTemplate.BackgroundTask.DomainEventHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloWorldDomainEventHandler : INotificationHandler<HelloWorldDomainEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(HelloWorldDomainEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Hello {notification}");
        }
    }
}
