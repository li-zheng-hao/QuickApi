using System;

namespace QuickApi.HttpResponse
{
    /// <summary>
    /// 需要忽略包装的函数上加这个特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]
    public class IgnoreResponseWrapperAttribute:Attribute
    {
        
    }
}