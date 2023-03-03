using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB.Repository;

public interface IMongoRepository<TDocument>  where TDocument : ModelBase
{
    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IMongoRepository<T> ChangeRepository<T>() where T:ModelBase;

    #region 查询

    /// <summary>
    /// 根据条件查询数量
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<int> GetCountAsync(Expression<Func<TDocument, bool>>? filter = null);

    /// <summary>
    /// 根据id字段批量获取文档
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<List<TDocument>> GetByIdsAsync(IEnumerable<string> ids);

    /// <summary>
    /// 根据id字段批量获取文档 DTO类
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<List<T>> GetByIdsAsync<T>(IEnumerable<string> ids);

    /// <summary>
    /// 根据ID字段获取文档
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TDocument> GetByIdAsync(string id);

    /// <summary>
    /// 根据ID字段获取文档 DTO
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetByIdAsync<T>(string id);

    /// <summary>
    /// 根据条件获取第一个数据
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderType"></param>
    /// <returns></returns>
    Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>>? filter = null,
        Expression<Func<TDocument, object>>? orderBy = null, Order? orderType = null);

    /// <summary>
    /// 异步分页查询
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    /// <param name="filter">过滤条件</param>
    /// <param name="pageNumber">索引[1,MAX]</param>
    /// <param name="pageSize">页数大小</param>
    /// <param name="option">mongo查询Option</param>
    Task<PageList<TDocument>> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null
    );
    /// <summary>
    /// 异步分页查询
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    /// <param name="filter">过滤条件</param>
    /// <param name="pageNumber">索引[1,MAX]</param>
    /// <param name="pageSize">页数大小</param>
    /// <param name="option">mongo查询Option</param>
    Task<PageList<T>> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter, int pageNumber = 1,
        int pageSize = 50
    );
    /// <summary>
    /// 异步分页查询
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderType"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="option"></param>
    Task<PageList<TDocument>> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter,
        Expression<Func<TDocument, object>>? orderBy, Order orderType = Order.Descending, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null);

    /// <summary>
    /// 异步分页查询
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    /// <typeparam name="T">DTO类</typeparam>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderType"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="option"></param>
    Task<PageList<T>> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter,
        Expression<Func<TDocument, object>>? orderBy=null, Order orderType = Order.Descending, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null);
    /// <summary>
    /// 异步分页查询 默认只查询前50条
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    Task<PageList<TDocument>> GetPaginatedAsync(FilterDefinition<TDocument> filter,
        Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null);

    /// <summary>
    /// 异步分页查询 默认只查询前50条
    /// </summary>
    /// <typeparam name="TDocument">实体类型</typeparam>
    /// <typeparam name="T">DTO类型</typeparam>
    Task<PageList<T>> GetPaginatedAsync<T>(FilterDefinition<TDocument> filter,
        Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending, int pageNumber = 1,
        int pageSize = 50, Action<AggregateOptions>? option = null);

    Task<List<TDocument>> QueryAsync(Expression<Func<TDocument, bool>> filter);

    /// <summary>
    /// T为DTO类
    /// </summary>
    /// <param name="filter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<List<T>> QueryAsync<T>(Expression<Func<TDocument, bool>> filter);

    Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> filter);

    /// <summary>
    /// T为DTO类
    /// </summary>
    /// <param name="filter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<List<T>> QueryAsync<T>(FilterDefinition<TDocument> filter);

    /// <summary>
    /// 获取一个从1开始的序列号 原子操作
    /// </summary>
    /// <param name="sequenceName"></param>
    /// <returns></returns>
    Task<ulong> GetNextSequenceNumberAsync(string sequenceName);

    #endregion

    #region 更新
    /// <summary>
    /// 仅仅更新一个字段
    /// </summary>
    /// <param name="members"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, TDocument entity);
    Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, List<TDocument> entities);
    
    /// <summary>
    /// 根据Id更新文档
    /// </summary>
    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
    /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
    Task<bool> UpdateOneAsync(TDocument modifiedDocument);

    /// <summary>
    /// 根据Id更新文档
    /// </summary>
    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
    /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
    Task<bool> UpdateOneAsync(string id,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

    /// <summary>
    /// 根据更新条件更新一个文档
    /// </summary>
    /// <param name="documentToModify"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    Task<bool> UpdateOneAsync(TDocument documentToModify,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

    /// <summary>
    /// 根据主键批量更新多个文件 集合为空则不更新并返回true
    /// </summary>
    /// <param name="documentsToModify"></param>
    /// <returns></returns>
    Task<bool> UpdateManyAsync(IEnumerable<TDocument> documentsToModify);

    /// <summary>
    /// 根据更新条件更新多个文档
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="updateDefinition"></param>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    Task<bool> UpdateManyAsync(FilterDefinition<TDocument> filter,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

    /// <summary>
    /// 根据更新条件更新多个文档
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="updateDefinition"></param>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    Task<bool> UpdateManyAsync(Expression<Func<TDocument, bool>> filter,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation);

    /// <summary>
    /// 根据更新条件更新多个文档
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="updateDefinition"></param>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    Task<bool> UpdateManyAsync(Expression<Func<TDocument, bool>> filter,
        UpdateDefinition<TDocument> updateDefinition);

    #endregion

    #region 删除

    /// <summary>
    /// 根据条件删除文档
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(FilterDefinition<TDocument> filter);
    /// <summary>
    /// 根据条件删除文档
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> filter);
    /// <summary>
    /// 根据条件删除文档
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filter);

    /// <summary>
    /// 根据id删除文档
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<bool> DeleteByIdAsync(string Id);

    #endregion

    #region 新增

    /// <summary>
    /// 新增文档
    /// </summary>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddOneAsync(TDocument document, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增多个文档
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddManyAsync(IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default);

    #endregion
}