using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using QuickApi.HttpResponse;
using QuickApi.UnitOfWork;
using QuickApi.UnitOfWork.MongoDB.Repository;
using QuickApi.WebapiSample.Model;

namespace QuickApi.WebapiSample.Controllers;

[ApiController]
[Route("[controller]")]
[IgnoreResponseWrapper]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ICapPublisher _capPublisher;
    private readonly IMongoRepository<MongoModel> _mongoRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,ICapPublisher capPublisher,IMongoRepository<MongoModel> mongoRepository)
    {
        _mongoRepository = mongoRepository;
        _capPublisher = capPublisher;
        _logger = logger;
    }
    [HttpGet(Name = "get1")]
    public string Get()
    {
        return "123";
    }
    
    [HttpGet("get2")]
    public Task<string> Get2()
    {
        return Task.FromResult("123");
    }
    [HttpGet("get3"),UnitOfWork(WithCap = true)]
    public async Task<string> Get3()
    {
        await _capPublisher.PublishAsync<string>("hello","hello");
        await _mongoRepository.AddOneAsync(new MongoModel() { UserName = "hello" });
        // throw new Exception("ex");
        return "123";
    }

    [NonAction, CapSubscribe("hello")]
    public void Test(string str)
    {
        _logger.LogInformation("接收到了消息"+str);
    }
}