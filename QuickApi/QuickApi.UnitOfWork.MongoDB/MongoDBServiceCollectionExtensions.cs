using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using QuickApi.UnitOfWork;
using QuickApi.UnitOfWork.MongoDB;
using QuickApi.UnitOfWork.MongoDB.Repository;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// https://stackoverflow.com/questions/2194047/net-best-practices-for-mongodb-connections
/// </summary>
public static class MongoDBServiceCollectionExtensions
{
    /// <summary>
    /// 添加MongoDB服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="database">数据库名</param>
    /// <param name="settings">设置</param>
    public static IServiceCollection AddMongoDB(this IServiceCollection services,string database,MongoClientSettings settings)
    {
        DB.InitAsync(database,settings);
        services.AddScoped<IUnitOfWork, MongoDBUnitOfWork>();
        services.AddScoped<IMongoUnitOfWork, MongoDBUnitOfWork>();
        services.AddScoped(typeof(IMongoRepository<>),typeof( MongoRepository<>));
        return services;
    }
    /// <summary>
    /// 添加MongoDB服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="database">数据库名</param>
    /// <param name="settings">设置</param>
    public static IServiceCollection AddMongoDB(this IServiceCollection services,string database,string host="127.0.0.1",int port=27017)
    {
        DB.InitAsync(database,host,port);
        services.AddScoped<IUnitOfWork, MongoDBUnitOfWork>();
        services.AddScoped<IMongoUnitOfWork, MongoDBUnitOfWork>();
        services.AddScoped(typeof(IMongoRepository<>),typeof( MongoRepository<>));
        return services;
    }
}