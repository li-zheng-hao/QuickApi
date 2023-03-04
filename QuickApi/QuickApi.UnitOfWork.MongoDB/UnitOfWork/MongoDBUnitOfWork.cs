using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB;


public class MongoDBUnitOfWork:IMongoUnitOfWork
{
    private int _enterCount = 0;
    
    public UnitOfWorkStatus UnitOfWorkStatus { get; set; }
    
    public Guid UnitOfWorkId { get; set; }
    
    public Transaction? Transaction { get; private set; }
    
    public ICapTransaction? CapTransaction { get; private set; }

    public MongoDBUnitOfWork()
    {
        UnitOfWorkId= Guid.NewGuid();
    }
    public Task BeginTransactionAsync(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        Interlocked.Increment(ref _enterCount);
        CheckCAPTransaction(context,unitOfWork);
        if (_enterCount > 1)
            return Task.CompletedTask;
        Transaction=DB.Transaction( default,new ClientSessionOptions()
        {
            DefaultTransactionOptions =new TransactionOptions(maxCommitTime:TimeSpan.FromSeconds(unitOfWork.TransactionTimeout))
        });
        CheckCAPTransaction(context,unitOfWork);
        UnitOfWorkStatus=UnitOfWorkStatus.Created;
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        Interlocked.Decrement(ref _enterCount);
        if (_enterCount == 0&&UnitOfWorkStatus==UnitOfWorkStatus.Created)
        {
            UnitOfWorkStatus = UnitOfWorkStatus.Committed;
            if(CapTransaction is not null)
                return CapTransaction.CommitAsync();
            return Transaction!.CommitAsync();
        }
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Rollbacked;
        if(CapTransaction is not null)
            return CapTransaction.RollbackAsync();
        return Transaction!.AbortAsync();
    }

    public void OnCompleted(FilterContext context, FilterContext resultContext)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Disposed;
        CapTransaction?.Dispose();
        Transaction?.Dispose();
    }
    
    private void CheckCAPTransaction(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        if (unitOfWork.WithCap&&CapTransaction==null&&Transaction!=null)
        {
            var publisher = context.HttpContext.RequestServices.GetService<ICapPublisher>();
            publisher.Transaction.Value = ActivatorUtilities.CreateInstance<MongoDBCapTransaction>(publisher.ServiceProvider);
            var capTrans = publisher.Transaction.Value.Begin(Transaction.Session, false);
            CapTransaction = capTrans;
        }
    }

   
}