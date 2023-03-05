using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using QuickApi.Internal;

namespace QuickApi.HttpResponse
{
    /// <summary>
    /// 返回值包装过滤器
    /// </summary>
    public class ResponseResultWrapperFilter:IActionFilter
    {
        private readonly ILogger<ResponseResultWrapperFilter>? _logger;
        
        public ResponseResultWrapperFilter(ILogger<ResponseResultWrapperFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var attributes = context.ActionDescriptor.EndpointMetadata.OfType<IgnoreResponseWrapperAttribute>();
            if (attributes.Any()||ReflectionHelper.TypeHasAttribute(context.Controller.GetType(),typeof(IgnoreResponseWrapperAttribute)))
                return;
            if (context.Exception is BusinessException businessExp)
            {
                context.HttpContext.Response.StatusCode = (int)businessExp.StatusCode;
                context.Result = new ObjectResult(new HttpResponseResult
                {
                    code = businessExp.BusinessCode,
                    message = businessExp.Message,
                    success = false
                });
            }
            else
            {
                if (context.Result is ObjectResult objectResult)
                {
                    if (objectResult.Value is HttpResponseResult)
                        return;
                    context.Result=new ObjectResult(new HttpResponseResult
                    {
                        code = 200,
                        success = true,
                        data = objectResult.Value
                    });
                }
                    
            }
        }
    }
}