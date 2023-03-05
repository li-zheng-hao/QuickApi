using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using QuickApi.SwaggerConfig;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerServiceCollectionExtension
{
    /// <summary>
    /// 自定义SwaggerUI页面
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="xmlPaths"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection serviceCollection,Assembly[]? xmlAssemblies=null)
    {
        // serviceCollection.AddSwaggerGenNewtonsoftSupport();
        serviceCollection.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();

        serviceCollection.AddSwaggerGen(c =>
        {

            #region Jwt头

            //添加Authorization
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "bearer",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string>()
                }
            });

            #endregion
          
            #region 接口文档抓取
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if(File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath, true);
            if (xmlAssemblies != null)
            {
                foreach (var assembly in xmlAssemblies)
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assembly.GetName().Name}.xml");
                    if (File.Exists(path))
                        c.IncludeXmlComments(path, false);
                }
            }
            #endregion
          
            //允许上传文件
            c.OperationFilter<FileUploadFilter>();
            // c.DocumentFilter<SwaggerIgnoreFilter>();

            #region 文档分组
            

            #endregion
        });
        return serviceCollection;
    }

    public static IApplicationBuilder UseCustomSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, req) =>
            {
                swagger.Servers = new List<OpenApiServer>() { new OpenApiServer() { Url = $"https://{req.Host}" } };
            });
        });
        var apiVersionDescriptionProvider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
        app.UseSwaggerUI(options =>
        {
            foreach (var desc in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.ApiVersion.ToString());
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            }
        });
        return app;
    }
}