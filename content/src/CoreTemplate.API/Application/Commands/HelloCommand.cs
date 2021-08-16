using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreTemplate.Domain.AggregatesModel;
#if (pg)
using CoreTemplate.Infrastructure.NpgSql;
#endif

namespace CoreTemplate.API.Application.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloCommand : IRequest<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HelloCommandHandler : IRequestHandler<HelloCommand, bool>
    {
#if (pg)
        private readonly InfrastructureContext _context;
#endif
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
#if (pg)
        /// <param name="context"></param>
#endif
        /// <param name="mapper"></param>
        public HelloCommandHandler(
#if (pg)
            InfrastructureContext context,
#endif
            IMapper mapper)
        {
#if (pg)
            _context = context ?? throw new ArgumentNullException(nameof(context));
#endif
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(HelloCommand request, CancellationToken cancellationToken)
        {
            var r = false;
#if (pg)
            var ent = _mapper.Map<AggregatesModelDemoEntity>(request);
            await _context.AggregatesModelDemoEntities.AddAsync(ent, cancellationToken);
            r= await _context.SaveEntitiesAsync(cancellationToken);
#endif
            return r;
        }
    }
}
