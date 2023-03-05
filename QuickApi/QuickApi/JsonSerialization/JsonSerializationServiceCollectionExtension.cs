using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using QuickApi.JsonSerialization;

namespace Microsoft.Extensions.DependencyInjection;

public class CustomJsonSerializationOption
{
    /// <summary>
    /// 属性序列化风格 默认驼峰 首字母小写
    /// </summary>
    public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

    /// <summary>
    /// 是否忽略空值 默认在写入null时忽略
    /// </summary>
    public JsonIgnoreCondition DefaultIgnoreCondition { get; set; } = JsonIgnoreCondition.Never;
    
    /// <summary>
    /// 默认序列化转换器
    /// </summary>
    public List<JsonConverter> JsonConverters=new()
    {
        new DefaultDateTimeConverter()
    };
}

public static class JsonSerializationServiceCollectionExtension
{
    /// <summary>
    /// 添加System.Text.Json序列化配置
    /// </summary>
    /// <param name="mvcBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IMvcBuilder AddCustomJsonSerialization(this IMvcBuilder mvcBuilder,
        Action<CustomJsonSerializationOption>? configure=null)
    {
        var config = new CustomJsonSerializationOption();
        configure?.Invoke(config);
        mvcBuilder.AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.DefaultIgnoreCondition = config.DefaultIgnoreCondition;
            configure.JsonSerializerOptions.PropertyNamingPolicy = config.PropertyNamingPolicy;
            foreach (var configJsonConverter in config.JsonConverters)
            {
                configure.JsonSerializerOptions.Converters.Add(configJsonConverter);
            }
        });
        return mvcBuilder;
    }
}