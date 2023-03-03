using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;

namespace QuickApi.UnitOfWork.SqlSugar;

/// <summary>
/// SqlSugar 工作单元实现
/// </summary>
public sealed class SqlSugarUnitOfWork : IUnitOfWork
{
    private int _enterCount = 0;
    public UnitOfWorkStatus UnitOfWorkStatus { get; set; }

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
    }


    /// <summary>
    /// 开启工作单元处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public Task BeginTransactionAsync(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        UnitOfWorkStatus=UnitOfWorkStatus.Created;
        Interlocked.Increment(ref _enterCount);
        return _sqlSugarClient.AsTenant().BeginTranAsync();
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
        UnitOfWorkStatus=UnitOfWorkStatus.Rollbacked;
        return _sqlSugarClient.AsTenant().RollbackTranAsync();
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
        _sqlSugarClient.Dispose();
    }
}