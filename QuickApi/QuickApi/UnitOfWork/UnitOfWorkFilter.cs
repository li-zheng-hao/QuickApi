using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuickApi.UnitOfWork
{
    public class UnitOfWorkFilterAttribute:ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext=await next();
            if (executedContext.Exception !=null)
            {
                
            }
        }
    }
}