using AutoMapper;
using CoreTemplate.API.Application.Commands;
using CoreTemplate.Domain.AggregatesModel;

namespace CoreTemplate.API.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class AutomapperConfigs : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public AutomapperConfigs()
        {
            CreateMap<HelloCommandHandler, AggregatesModelDemoEntity>()
              .ForMember(l => l.City, r => r.Ignore());
        }
    }
}
