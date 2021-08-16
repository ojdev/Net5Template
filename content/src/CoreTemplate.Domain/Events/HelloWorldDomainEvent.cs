using MediatR;
using System;

namespace CoreTemplate.Domain.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloWorldDomainEvent : INotification
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public HelloWorldDomainEvent(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
