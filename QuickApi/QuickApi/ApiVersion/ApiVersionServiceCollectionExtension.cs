using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;
/// <summary>
/// https://github.com/dotnet/aspnet-api-versioning/issues/614
/// </summary>
public static class ApiVersionServiceCollectionExtension
{
    /// <summary>
    /// 添加Api版本控制
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomApiVersion(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            // 从请求头中获取版本号
            options.ApiVersionReader= new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("api-version");
            options.ApiVersionSelector= new Microsoft.AspNetCore.Mvc.Versioning.CurrentImplementationApiVersionSelector(options);
            options.AssumeDefaultVersionWhenUnspecified = true;
            // 注释下面这行 默认选最新的版本号
            // options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        });
        serviceCollection.AddVersionedApiExplorer(setup =>
        {
        });
        return serviceCollection;
    }
}