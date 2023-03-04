using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace QuickApi.SwaggerConfig;

public static class SwaggerServiceCollectionExtension
{
    /// <summary>
    /// 自定义SwaggerUI页面
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="xmlPaths"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection serviceCollection,List<string> xmlPaths)
    {
        // serviceCollection.AddSwaggerGenNewtonsoftSupport();
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if(File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath, true);
            foreach (var path in xmlPaths)
            {
                if(File.Exists(path))
                    c.IncludeXmlComments(path, false);
            }

            #endregion
          
            //允许上传文件
            c.OperationFilter<FileUploadFilter>();
            // c.DocumentFilter<SwaggerIgnoreFilter>();

            #region 文档分组
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "公共模块",
                Description = "接口说明(多模式管理,右上角切换)",
            });
            
            // TODO  这里要添加其他模块的文档分组
            // c.SwaggerDoc(SwaggerGroup.自定义模块, new OpenApiInfo { Title = TypeHelper.GetFieldDescription(typeof(自定义模块),nameof(SwaggerGroup.自定义模块)),
            //      Version =SwaggerGroup.自定义模块 });
            
            //设置要展示的接口
            c.DocInclusionPredicate((docName, apiDes) =>
            {
                if (!apiDes.TryGetMethodInfo(out MethodInfo method))
                    return false;
                var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                if (docName == "v1" && !version.Any())
                    return true;
                //这里获取action的特性
                var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                if (actionVersion.Any())
                    return actionVersion.Any(v => v == docName);
                return version.Any(v => v == docName);
            });

            #endregion
        });
        return serviceCollection;
    }

    public static IApplicationBuilder UseCustomSwaggerUI(SwaggerUIOptions options, IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "公共模块");
            // TODO: 生成多个文档
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });
        return app;
    }
}