using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApi.Mapper
{
    public static class MapsterServiceCollectionExtensions
    {
        public static IServiceCollection AddMapster(this IServiceCollection serviceCollection,Assembly configAssembly)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            // 全局忽略大小写
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
            Assembly applicationAssembly = typeof(BaseDto<,>).Assembly;
            typeAdapterConfig.Scan(applicationAssembly);
            typeAdapterConfig.Scan(configAssembly);
            return serviceCollection;
        }
    }
}