using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using QuickApi.HttpResponse;

namespace QuickApi.UnitTest.HttpResponseFilterTests;

public partial class TestCases
{
    [Fact]
    public void ResponseResultWrapperFilter_ShouldReplaceResult()
    {
        var logger=Mock.Of<ILogger<ResponseResultWrapperFilter>>();
        // Arrange (Prepare the objects and set the values of the data used in the test)
        // Create a default ActionContext (depending on our case-scenario)
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new List<object>()
            }
        };

       
        ActionExecutedContext actionExecutedContext = new ActionExecutedContext(
            actionContext,
    new List<IFilterMetadata>(),
            typeof(TestCases.TestController)
        );
        actionExecutedContext.Result = new ObjectResult("test");
        ResponseResultWrapperFilter addHeaderAttribute = new ResponseResultWrapperFilter(logger);
        addHeaderAttribute.OnActionExecuted(actionExecutedContext);
        ObjectResult result = new ("origin");
        var changedResult=actionExecutedContext.Result as ObjectResult;
        Assert.True(changedResult!.Value is HttpResponseResult);

    }

    [Fact]
    public void ResponseResultWrapperFilter_ShouldReplaceResultWithException()
    {
        var logger=Mock.Of<ILogger<ResponseResultWrapperFilter>>();
        // Arrange (Prepare the objects and set the values of the data used in the test)
        // Create a default ActionContext (depending on our case-scenario)
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new List<object>()
            }
        };

       
        ActionExecutedContext actionExecutedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            new TestController()
        );
        actionExecutedContext.Exception = new BusinessException("error");
        
        ResponseResultWrapperFilter addHeaderAttribute = new ResponseResultWrapperFilter(logger);
        addHeaderAttribute.OnActionExecuted(actionExecutedContext);
        ObjectResult result = new ("origin");
        var changedResult=actionExecutedContext.Result as ObjectResult;
        Assert.True(changedResult!.Value is HttpResponseResult);
        var value=changedResult.Value as HttpResponseResult;
        Assert.True(value!.success==false);
        Assert.True(value.message=="error");

    }
    
    [Fact]
    public void ResponseResultWrapperFilter_ShouldIgnore()
    {
        var logger=Mock.Of<ILogger<ResponseResultWrapperFilter>>();
        // Arrange (Prepare the objects and set the values of the data used in the test)
        // Create a default ActionContext (depending on our case-scenario)
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new List<object>()
                {
                    new IgnoreResponseWrapperAttribute()
                }
            }
        };

       
        ActionExecutedContext actionExecutedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            new TestController()
        );
        actionExecutedContext.Result = new ObjectResult("test");
        ResponseResultWrapperFilter addHeaderAttribute = new ResponseResultWrapperFilter(logger);
        addHeaderAttribute.OnActionExecuted(actionExecutedContext);
        ObjectResult result = new ("origin");
        var changedResult=actionExecutedContext.Result as ObjectResult;
        Assert.False(changedResult!.Value is HttpResponseResult);

    }
    [Fact]
    public void ResponseResultWrapperFilter_ShouldIgnore2()
    {
        var logger=Mock.Of<ILogger<ResponseResultWrapperFilter>>();
        // Arrange (Prepare the objects and set the values of the data used in the test)
        // Create a default ActionContext (depending on our case-scenario)
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new List<object>()
            }
        };
        ActionExecutedContext actionExecutedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            new TestControllerWithIgnoreAttribute()
        );
        actionExecutedContext.Result = new ObjectResult("test");
        ResponseResultWrapperFilter addHeaderAttribute = new ResponseResultWrapperFilter(logger);
        addHeaderAttribute.OnActionExecuted(actionExecutedContext);
        ObjectResult result = new ("origin");
        var changedResult=actionExecutedContext.Result as ObjectResult;
        Assert.False(changedResult!.Value is HttpResponseResult);

    }
}