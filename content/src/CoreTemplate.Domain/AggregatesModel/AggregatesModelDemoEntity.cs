#if (pg)
using Microsoft.EntityFrameworkCore;
#endif
using System;
#if (pg)
using CoreTemplate.Domain.Events;
using CoreTemplate.Domain.SeedWork;
#endif

namespace CoreTemplate.Domain.AggregatesModel
{
    /// <summary>
    /// 
    /// </summary>
#if (pg)
    [Comment("数据库注释")]
#endif
    public class AggregatesModelDemoEntity
#if (pg)
        : Entity
#endif
    {
        /// <summary>
        /// 
        /// </summary>
#if (pg)
        [Comment("数据库注释")]
#endif
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
#if (pg)
        [Comment("数据库注释")]
#endif
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AggregatesModelDemoEntity()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="name"></param>
        public AggregatesModelDemoEntity(string city, string name)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            Name = name ?? throw new ArgumentNullException(nameof(name));
#if (pg)
            AddDomainEvent(new HelloWorldDomainEvent(Name));
#endif
        }
    }
}
