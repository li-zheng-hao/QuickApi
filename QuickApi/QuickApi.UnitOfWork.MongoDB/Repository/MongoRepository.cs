using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB.Repository;
/// <summary>
/// 仓储层基类，一些公用的方法添加在这里
/// </summary>
/// <typeparam name="TDocument"></typeparam>
public class MongoRepository<TDocument> :IMongoRepository<TDocument>  where TDocument:ModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MongoDBUnitOfWork _uow;
    
    public MongoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _uow=serviceProvider.GetService<IUnitOfWork>() as MongoDBUnitOfWork;

    }

    public IMongoRepository<T> ChangeRepository<T>() where T : ModelBase
    {
        return _serviceProvider.GetService<IMongoRepository<T>>();
    }

    public async Task<int> GetCountAsync(Expression<Func<TDocument, bool>>? filter = null)
    {
        
        long count = 0;
        var collection = DB.Collection<TDocument>();
        if (filter != null&&_uow.Transaction!=null)
        {
            count=await DB.Collection<TDocument>().CountDocumentsAsync(_uow.Transaction?.Session,filter);
        }
        else if (_uow.Transaction == null&&filter!=null)
        {
            count=await DB.Collection<TDocument>().CountDocumentsAsync(filter);

        }
        else if(filter==null&&_uow.Transaction!=null)
        {
            count=await DB.Collection<TDocument>().CountDocumentsAsync(_uow.Transaction?.Session,it=>true);
        }
        else
        {
            count=await DB.Collection<TDocument>().CountDocumentsAsync(it=>true);

        }
        return (int)count;
    }

    public async Task<List<TDocument>> GetByIdsAsync(IEnumerable<string> ids)
    {
        return await DB.Find<TDocument>(_uow.Transaction?.Session).Match(filter => filter.In(
            it=>it.ID,ids)).ExecuteAsync();
    }

    public Task<List<T>> GetByIdsAsync<T>(IEnumerable<string> ids)
    {
        return  DB.Find<TDocument,T>(_uow.Transaction?.Session).Match(filter => filter.In(
            it=>it.ID,ids)).ExecuteAsync();
    }

    public async Task<TDocument> GetByIdAsync(string id)
    {
        return await DB.Find<TDocument>(_uow.Transaction?.Session).OneAsync(id);
    }

    public Task<T> GetByIdAsync<T>(string id)
    {
        return DB.Find<TDocument,T>(_uow.Transaction?.Session).OneAsync(id);
    }

    public async Task<TDocument> GetFirstAsync(Expression<Func<TDocument, bool>>? filter = null, Expression<Func<TDocument, object>>? orderBy = null, Order? orderType = null)
    {
        var pipeline=DB.Find<TDocument>(_uow.Transaction?.Session);
        if(filter!=null)
        {
            pipeline.Match(filter);
        }
        if (orderBy != null)
            pipeline.Sort(orderBy, orderType!.Value);
        return await pipeline.ExecuteFirstAsync();
    }

    public async Task<PageList<TDocument>> GetPaginatedAsync(Expression<Func<TDocument, bool>>? filter, int pageNumber = 1, int pageSize = 50,Action<AggregateOptions>? option=null)
    {
        var pipeline=DB.PagedSearch<TDocument>(_uow.Transaction?.Session);
        if(filter!=null)
            pipeline.Match(filter);
        if (option != null)
        {
            pipeline.Option(option);
        }
        var res=await pipeline.Sort(b => b.ID, Order.Ascending).PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();

        var pagelist= new PageList<TDocument>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
        return pagelist;
    }

    public async Task<PageList<T>> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter, int pageNumber = 1, int pageSize = 50)
    {
        var pipeline= DB.PagedSearch<TDocument,T>(_uow.Transaction?.Session);
        if(filter!=null)
            pipeline.Match(filter);
        pipeline.Sort(it => it.ID, Order.Ascending);
        var res=await pipeline.ProjectExcluding(_=>new{_._dummyProp}).PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();
        return new PageList<T>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
    }

    public async Task<PageList<TDocument>> GetPaginatedAsync(Expression<Func<TDocument, bool>> filter
        , Expression<Func<TDocument, object>>? orderBy, Order orderType, int pageNumber = 1,
        int pageSize = 50,Action<AggregateOptions>? option=null)
    {
        var pipeline= DB.PagedSearch<TDocument>(_uow.Transaction?.Session).Match(filter);
        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }
        var res=await pipeline.PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();
        return new PageList<TDocument>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
    }

    public async Task<PageList<T>> GetPaginatedAsync<T>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, object>>? orderBy=null, Order orderType = Order.Descending,
        int pageNumber = 1, int pageSize = 50, Action<AggregateOptions>? option = null)
    {
        var pipeline= DB.PagedSearch<TDocument,T>(_uow.Transaction?.Session).Match(filter);
        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }
        var res=await pipeline.ProjectExcluding(_=>new{_._dummyProp}).PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();
        return new PageList<T>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
    }

    public async Task<PageList<TDocument>> GetPaginatedAsync(FilterDefinition<TDocument> filter,Expression<Func<TDocument,object>>? orderBy=null,Order orderType=Order.Descending, int pageNumber = 1,
        int pageSize = 50,Action<AggregateOptions>? option=null)
    {
        var pipeline= DB.PagedSearch<TDocument>(_uow.Transaction?.Session).Match(filter);
        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }
        var res=await pipeline.PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();
        return new PageList<TDocument>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
    }

    public async Task<PageList<T>> GetPaginatedAsync<T>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, object>>? orderBy = null, Order orderType = Order.Descending,
        int pageNumber = 1, int pageSize = 50, Action<AggregateOptions>? option = null)
    {
        var pipeline= DB.PagedSearch<TDocument,T>(_uow.Transaction?.Session).Match(filter);
        if (orderBy == null)
        {
            pipeline.Sort(it => it.ID, Order.Ascending);
        }
        else
        {
            pipeline.Sort(orderBy, orderType);
        }

        if (option != null)
        {
            pipeline.Option(option);
        }
        var res=await pipeline.ProjectExcluding(_=>new{_._dummyProp}).PageSize(pageSize).PageNumber(pageNumber).ExecuteAsync();
        return new PageList<T>(res.Results.ToList(), pageNumber, pageSize,(int)res.TotalCount);
    }

    public async Task<List<TDocument>> QueryAsync(Expression<Func<TDocument, bool>> filter)
    {
        var res=await DB.Find<TDocument>(_uow.Transaction?.Session).Match(filter).ExecuteAsync();
        return res;
    }

    public async Task<List<T>> QueryAsync<T>(Expression<Func<TDocument, bool>> filter)
    {
        var res=await DB.Find<TDocument,T>().Match(filter).ExecuteAsync();
        return res;
    }
    public async Task<List<T>> QueryAsync<T>(FilterDefinition<TDocument> filter)
    {
        var res=await DB.Find<TDocument,T>().Match(filter).ExecuteAsync();
        return res;
    }

    public async Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> filter)
    {
        var res=await DB.Find<TDocument>().Match(filter).ExecuteAsync();
        return res;
    }

    public async Task<ulong> GetNextSequenceNumberAsync(string sequenceName)
    {
        return await DB.NextSequentialNumberAsync(sequenceName);
    }

    public async Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, TDocument entity)
    {
        var res=await DB.Update<TDocument>(_uow.Transaction?.Session).MatchID(entity.ID).ModifyOnly(members,entity).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateOnlyAsync(Expression<Func<TDocument, object>> members, List<TDocument> entities)
    {
        var res=await DB.SaveOnlyAsync(entities, members, _uow.Transaction?.Session);
        return res.MatchedCount > 0;
    }


    public async Task<bool> UpdateOneAsync(TDocument modifiedDocument)
    {
        // var res=await DB.Find<TDocument>().Match(it=>it.ID==modifiedDocument.ID).ExecuteAsync();
        var res=await DB.Replace<TDocument>(_uow.Transaction?.Session).MatchID(modifiedDocument.ID).WithEntity(modifiedDocument).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateOneAsync(string id, Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(_uow.Transaction?.Session).MatchID(id).Modify(operation).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateOneAsync(TDocument documentToModify, Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(_uow.Transaction?.Session).MatchID(documentToModify.ID).Modify(operation).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateManyAsync(IEnumerable<TDocument> documentsToModify)
    {
        if(!documentsToModify.Any())
            return true;
        var res=await documentsToModify.SaveAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateManyAsync(FilterDefinition<TDocument> filter, Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(_uow.Transaction?.Session).Match(filter).Modify(operation).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateManyAsync(Expression<Func<TDocument, bool>> filter, Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> operation)
    {
        var res = await DB.Update<TDocument>(_uow.Transaction?.Session).Match(filter).Modify(operation).ExecuteAsync();
        return res.MatchedCount>0;
    }

    public async Task<bool> UpdateManyAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> updateDefinition)
    {
        
        var res = await DB.Update<TDocument>(_uow.Transaction?.Session).Match(filter).Modify(it=>updateDefinition).ExecuteAsync();
        return res.MatchedCount>0;
    }


    public async Task<bool> DeleteAsync(FilterDefinition<TDocument> filter)
    {
        var res = await DB.DeleteAsync<TDocument>(filter,_uow.Transaction?.Session);
        return res.DeletedCount > 0;
    }

    public async Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> filter)
    {
        var res = await DB.DeleteAsync<TDocument>(filter,_uow.Transaction?.Session);
        return res.DeletedCount > 0;
    }

    public async Task<bool> DeleteAsync(Func<FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filter)
    {
        var res=await DB.DeleteAsync<TDocument>(filter);
        return res.DeletedCount>0;
    }

    public async Task<bool> DeleteByIdAsync(string Id)
    {
        var res = await DB.DeleteAsync<TDocument>(Id,_uow.Transaction?.Session);
        return res.DeletedCount > 0;
    }

    public async Task AddOneAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        await DB.SaveAsync<TDocument>(document,_uow.Transaction?.Session);
    }

    public async  Task AddManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        await DB.SaveAsync<TDocument>(documents,_uow.Transaction?.Session);

    }
}