﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using CoreTemplate.API.Infrastructure.Models;

namespace CoreTemplate.API.Infrastructure.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiResponseFilterAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult @object)
            {
                context.Result = new ObjectResult(new ApiResponse<object>(result: @object?.Value));
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new ApiResponse<object>());
            }
            else if (context.Result is JsonResult json)
            {
                context.Result = new ObjectResult(new ApiResponse<object>(result: json?.Value));
            }
            else if (context.Result is ContentResult content)
            {
                context.Result = new ObjectResult(new ApiResponse<object>(result: content?.Content));
            }
            else if (context.Result is StatusCodeResult status)
            {
                context.Result = new ObjectResult(new ApiResponse<object>());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
