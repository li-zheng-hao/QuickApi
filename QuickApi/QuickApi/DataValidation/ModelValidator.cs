using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using QuickApi.HttpResponse;

namespace QuickApi.DataValidation;

/// <summary>
/// 请求验证错误处理
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class ModelValidator : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext actionContext)
    {
        var modelState = actionContext.ModelState;
        if (!modelState.IsValid)
        {
            var baseResult = new HttpResponseResult()
            {
                success = false,
                code= 400,
                message= "参数错误",
            };
            List<string> errors = new List<string>();
            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];
                if (state.Errors.Any())
                {
                    errors.Add( state.Errors?.FirstOrDefault()?.ErrorMessage);
                }
            }
            var firstErr=modelState.FirstOrDefault(it => it.Value != null && it.Value.Errors.Any()).Value?.Errors.First()
                .ErrorMessage;
            // 只列出第一个错误
            if(!string.IsNullOrWhiteSpace(firstErr))
                baseResult.message=firstErr!;
            baseResult.data = errors;
            actionContext.Result = new ContentResult
            {
                Content = JsonConvert.SerializeObject(baseResult),
                ContentType = "application/json"
            };
        }
    }
}
