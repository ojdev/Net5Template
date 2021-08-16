using FluentValidation;
using Microsoft.Extensions.Logging;
using CoreTemplate.API.Application.Commands;

namespace CoreTemplate.API.Application.Validations
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloCommandValidator : AbstractValidator<HelloCommand>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public HelloCommandValidator(ILogger<HelloCommandValidator> logger)
        {
            RuleFor(order => order.Name).NotEmpty().WithMessage("不允许为空。");
        }
    }
}
