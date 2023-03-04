using System.Transactions;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace QuickApi.UnitOfWork.SqlSugar;

/// <summary>
/// SqlSugar 工作单元实现
/// </summary>
public sealed class SqlSugarUnitOfWork : IUnitOfWork
{
    private int _enterCount = 0;
    
    public ICapTransaction? CapTransaction { get;private set; }
    public UnitOfWorkStatus UnitOfWorkStatus { get; set; }
    public Guid UnitOfWorkId { get; set; }

    /// <summary>
    /// SqlSugar 对象
    /// </summary>
    private readonly ISqlSugarClient _sqlSugarClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    public SqlSugarUnitOfWork(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
        UnitOfWorkId = new Guid();
    }


    /// <summary>
    /// 开启工作单元处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task BeginTransactionAsync(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        Interlocked.Increment(ref _enterCount);
        CheckCAPTransaction(context, unitOfWork);
        if (_enterCount > 1)
            return ;
        UnitOfWorkStatus=UnitOfWorkStatus.Created;
        await _sqlSugarClient.AsTenant().BeginTranAsync();
        CheckCAPTransaction(context, unitOfWork);
    }

    /// <summary>
    /// 提交工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public Task CommitTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        Interlocked.Decrement(ref _enterCount);
        if (_enterCount == 0&&UnitOfWorkStatus==UnitOfWorkStatus.Created)
        {
            UnitOfWorkStatus = UnitOfWorkStatus.Committed;
            if (CapTransaction != null)
                return CapTransaction.CommitAsync();
            return _sqlSugarClient.AsTenant().CommitTranAsync();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 回滚工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public Task RollbackTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        if (UnitOfWorkStatus == UnitOfWorkStatus.Created)
        {
            UnitOfWorkStatus=UnitOfWorkStatus.Rollbacked;
            if (CapTransaction != null)
                return CapTransaction.RollbackAsync();
            return _sqlSugarClient.AsTenant().RollbackTranAsync();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 执行完毕（无论成功失败）
    /// </summary>
    /// <param name="context"></param>
    /// <param name="resultContext"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnCompleted(FilterContext context, FilterContext resultContext)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Disposed;
        CapTransaction?.Dispose();
        // 必须要设置为null，否则sqlsugar内部会再次rollback
        _sqlSugarClient.Ado.Transaction = null;
        _sqlSugarClient.Dispose();
    }

    private void CheckCAPTransaction(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        if (unitOfWork.WithCap&&CapTransaction==null&&_sqlSugarClient.Ado.Transaction!=null)
        {
            var publisher = context.HttpContext.RequestServices.GetService<ICapPublisher>();
            publisher.Transaction.Value = ActivatorUtilities.CreateInstance<MySqlCapTransaction>(publisher.ServiceProvider);
            var capTrans = publisher.Transaction.Value.Begin(_sqlSugarClient.Ado.Transaction, false);
            CapTransaction = capTrans;
        }
    }
}