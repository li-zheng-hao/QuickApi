using System.Security.Claims;
using DotNetCore.CAP;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickApi.HttpResponse;
using QuickApi.JwtAuthorization;
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
    private readonly JwtTokenManager _tokenManager;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,ICapPublisher capPublisher,IMongoRepository<MongoModel> mongoRepository,JwtTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
        _mongoRepository = mongoRepository;
        _capPublisher = capPublisher;
        _logger = logger;
    }

    [HttpGet("login")]
    public string Login()
    {
        var token = _tokenManager.CreateToken(new List<Claim>()
        {
            new Claim(JwtClaimTypes.Role,"admin"),
            new Claim(JwtClaimTypes.Name,"testname")
        });
        _logger.LogInformation(token);
        return token;
    }
    [HttpGet("authorize"),Authorize(Roles = "admin")]
    public Task<string> Authorize()
    {
        var user=User.IsInRole("admin");
        return Task.FromResult("123");
    }
    
    [HttpGet("unitofwork"),UnitOfWork(WithCap = true)]
    public async Task<string> UnitOfWork()
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