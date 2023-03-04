using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using QuickApi.UnitOfWork;
using QuickApi.WebapiSample.Sqlsugar.Mysql.Model;
using SqlSugar;

namespace QuickApi.WebapiSample.Sqlsugar.Mysql.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISqlSugarRepository<SqlSugarModel> _sqlSugarRepository;
    private readonly ICapPublisher _capPublisher;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,ICapPublisher capPublisher,ISqlSugarRepository<SqlSugarModel> sqlSugarRepository)
    {
        _capPublisher = capPublisher;
        _sqlSugarRepository = sqlSugarRepository;
        _logger = logger;
    }

    [HttpGet("test"),UnitOfWork(WithCap = true)]
    public async Task<string> Test()
    {
        await _capPublisher.PublishAsync<string>("hello","hello");
        await _sqlSugarRepository.InsertAsync(new SqlSugarModel() { Name = "hello",SchoolId = 123});
        return "123";
    }

    [NonAction, CapSubscribe("hello")]
    public void Test(string str)
    {
        _logger.LogInformation("接收到了消息"+str);
    }
}