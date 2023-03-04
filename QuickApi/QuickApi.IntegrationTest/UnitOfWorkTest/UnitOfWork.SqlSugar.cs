using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using QuickApi.UnitOfWork;
using Savorboard.CAP.InMemoryMessageQueue;
using SqlSugar;

namespace QuickApi.IntegrationTest.UnitOfWorkTest;

public class UnitOfWork_SqlSugar
{
    private readonly ServiceProvider _sp;
    private readonly ISqlSugarClient _client;

    public UnitOfWork_SqlSugar()
    {
        ServiceCollection sc = new ServiceCollection();
        // sc.AddLogging();
        // sc.AddCap(x =>
        // {
        //     x.UseMySql(DBConnectionStrings.MysqlConnectionString);
        //     x.UseInMemoryMessageQueue();
        // });
        sc.AddSqlSugar(new ConnectionConfig()
            {
                ConnectionString = DBConnectionStrings.MysqlConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            },
            db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql); //输出sql,查看执行sql 性能无影响
                };
            });
        _sp = sc.BuildServiceProvider();
        _client = _sp.GetRequiredService<ISqlSugarClient>();
        _client.DbMaintenance.CreateDatabase();
        _client.CodeFirst.InitTables(typeof(SqlSugarModel));
    }

    [Fact]
    public async Task Test()
    {
        var sqlsugarUow = _sp.GetService<IUnitOfWork>();
        FilterContext context = default;
        UnitOfWorkAttribute unitOfWork = new UnitOfWorkAttribute();
        await sqlsugarUow.BeginTransactionAsync(context, unitOfWork);
        var repository = _sp.GetService<ISqlSugarRepository<SqlSugarModel>>();
        var test = new SqlSugarModel { Name = "test", SchoolId = 123 };
        await repository.InsertAsync(test);
        await sqlsugarUow.RollbackTransactionAsync(context, unitOfWork);
        var result = await repository.CountAsync(it => true);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task TestWithWrap()
    {
        var sqlsugarUow = _sp.GetService<IUnitOfWork>();
        FilterContext context = default;
        UnitOfWorkAttribute unitOfWork = new UnitOfWorkAttribute();
        await sqlsugarUow.BeginTransactionAsync(context, unitOfWork);
        await sqlsugarUow.BeginTransactionAsync(context, unitOfWork);
        var repository = _sp.GetService<ISqlSugarRepository<SqlSugarModel>>();
        var test = new SqlSugarModel { Name = "test", SchoolId = 123 };
        await repository.InsertAsync(test);
        await sqlsugarUow.CommitTransactionAsync(context, unitOfWork);
        // 仍然未提交
        Assert.Equal(UnitOfWorkStatus.Created, sqlsugarUow.UnitOfWorkStatus);
        await sqlsugarUow.RollbackTransactionAsync(context, unitOfWork);
        var result = await repository.CountAsync(it => true);
        Assert.Equal(0, result);
    }
   
}

public class SqlSugarModel
{
    //数据是自增需要加上IsIdentity 
    //数据库是主键需要加上IsPrimaryKey 
    //注意：要完全和数据库一致2个属性
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public int? SchoolId { get; set; }
    public string Name { get; set; }
}