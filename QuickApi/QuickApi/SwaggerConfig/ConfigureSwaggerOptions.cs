using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuickApi.SwaggerConfig;

public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerConfigureOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = $"版本号：{desc.ApiVersion} - QuickApi",
                Version = desc.ApiVersion.ToString(),
                Description = desc.IsDeprecated ? "该版本已经被标记为抛弃,请使用最新的api!":"",
            });
        }
    }
}