using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using CoreTemplate.API.Application.Commands;
using CoreTemplate.API.Infrastructure.Models;

namespace CoreTemplate.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldontroller : ControllerBase
    {
        private readonly ILogger<HelloWorldontroller> _logger;
        private readonly IMediator _mediator;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public HelloWorldontroller(ILogger<HelloWorldontroller> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] HelloCommand command)
        {
            await _mediator.Send(command);
            return Ok("hello world!");
        }
    }
}
