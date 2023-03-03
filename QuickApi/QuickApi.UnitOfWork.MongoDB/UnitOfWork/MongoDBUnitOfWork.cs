using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB;


public class MongoDBUnitOfWork:IMongoUnitOfWork
{
    private int _enterCount = 0;
    public UnitOfWorkStatus UnitOfWorkStatus { get; set; }

    public Task BeginTransactionAsync(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        Transaction=DB.Transaction( default,new ClientSessionOptions()
        {
            DefaultTransactionOptions =new TransactionOptions(maxCommitTime:TimeSpan.FromSeconds(unitOfWork.TransactionTimeout))
        });
        UnitOfWorkStatus=UnitOfWorkStatus.Created;
        Interlocked.Increment(ref _enterCount);
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        Interlocked.Decrement(ref _enterCount);
        if (_enterCount == 0&&UnitOfWorkStatus==UnitOfWorkStatus.Created)
        {
            UnitOfWorkStatus = UnitOfWorkStatus.Committed;
            return Transaction?.CommitAsync();
        }
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Rollbacked;
        return Transaction?.AbortAsync();
    }

    public void OnCompleted(FilterContext context, FilterContext resultContext)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Disposed;
        Transaction?.Dispose();
    }

    public Transaction? Transaction { get; private set; }
}