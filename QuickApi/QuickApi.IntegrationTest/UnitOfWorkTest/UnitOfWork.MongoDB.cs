using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using QuickApi.UnitOfWork;
using QuickApi.UnitOfWork.MongoDB;
using QuickApi.UnitOfWork.MongoDB.Repository;

namespace QuickApi.IntegrationTest.UnitOfWorkTest;

public class UnitOfWork_MongoDB:IDisposable
{
    private readonly ServiceProvider _sp;

    public UnitOfWork_MongoDB()
    {
        ServiceCollection sc = new ServiceCollection();
        sc.AddMongoDB("QuickApi",MongoClientSettings.FromConnectionString(
            DBConnectionStrings.MongoDBConnectionString));
        _sp=sc.BuildServiceProvider();
        var seed=new MongoTestModel()
        {
            TestField1 = "test",
            TestField2 = "test",
        };
        seed.SaveAsync().GetAwaiter().GetResult();
        seed.DeleteAsync().GetAwaiter().GetResult();
    }

   
    [Fact]
    public async Task Test()
    {
        var mongoUnitOfWork = _sp.GetService<IUnitOfWork>();
        FilterContext context = default;
        UnitOfWorkAttribute unitOfWork = new UnitOfWorkAttribute();
        await mongoUnitOfWork.BeginTransactionAsync(context, unitOfWork);
        var repository = _sp.GetService<IMongoRepository<MongoTestModel>>();
        var test=new MongoTestModel {TestField1 = "test1", TestField2 = "test1"};
        await repository.AddOneAsync(test);
        await mongoUnitOfWork.RollbackTransactionAsync(context, unitOfWork);
        var result = await repository.GetCountAsync();
        Assert.Equal(0,result);
    }
    
    [Fact]
    public async Task TestWithWrap()
    {
        var mongoUnitOfWork = _sp.GetService<IUnitOfWork>();
        FilterContext context = default;
        UnitOfWorkAttribute unitOfWork = new UnitOfWorkAttribute();
        await mongoUnitOfWork.BeginTransactionAsync(context, unitOfWork);
        await mongoUnitOfWork.BeginTransactionAsync(context, unitOfWork);
        var repository = _sp.GetService<IMongoRepository<MongoTestModel>>();
        var test=new MongoTestModel {TestField1 = "test1", TestField2 = "test1"};
        await repository.AddOneAsync(test);
        await mongoUnitOfWork.CommitTransactionAsync(context, unitOfWork);
        // 仍然未提交
        Assert.Equal(UnitOfWorkStatus.Created, mongoUnitOfWork.UnitOfWorkStatus);
        await mongoUnitOfWork.RollbackTransactionAsync(context, unitOfWork);
        var result = await repository.GetCountAsync();
        Assert.Equal(0,result);
    }
    public void Dispose()
    {
        _sp.Dispose();
    }
}


public class MongoTestModel : ModelBase
{
    public string TestField1 { get; set; }
    public string TestField2 { get; set; }
}