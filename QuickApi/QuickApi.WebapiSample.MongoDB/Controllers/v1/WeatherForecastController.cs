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

namespace QuickApi.WebapiSample.Controllers.V1;

[ApiController]
[ApiVersion("1.0", Deprecated = true)]
// 不需要版本控制
// [ApiVersionNeutral]
[Route("[controller]")]
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

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ICapPublisher capPublisher,
        IMongoRepository<MongoModel> mongoRepository, JwtTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
        _mongoRepository = mongoRepository;
        _capPublisher = capPublisher;
        _logger = logger;
    }
    /// <summary>
    /// Api版本
    /// </summary>
    /// <returns></returns>
    [HttpGet("apiversion")]
    public string ApiVersion1()
    {
        return "1.0";
    }
    
    /// <summary>
    /// 忽略返回值包装
    /// </summary>
    /// <returns></returns>
    [HttpGet("ignoreresponsewrapper"),IgnoreResponseWrapper]
    public string IgnoreResponseWrapper()
    {
        return "raw response";
    }
    /// <summary>
    /// 自定义json序列化
    /// </summary>
    /// <returns></returns>
    [HttpGet("JsonSerialize")]
    public PageList<dynamic> JsonSerialize()
    {
        var data = new { UserName = 1, Password = 2, age = 1, Time = DateTime.Now };
        PageList<dynamic> pageList = new PageList<dynamic>(new List<dynamic>() { data }, 1, 10, 1);
        return pageList;
    }
    /// <summary>
    /// 登录 获取token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public string Login([FromBody]LoginDto dto)
    {
        var token = _tokenManager.CreateToken(new List<Claim>()
        {
            new Claim(JwtClaimTypes.Role, "admin"),
            new Claim(JwtClaimTypes.Name, "testname")
        });
        _logger.LogInformation(token);
        return token;
    }
    /// <summary>
    /// 基于角色的授权 admin角色可访问
    /// </summary>
    /// <returns></returns>
    [HttpGet("authorize"), Authorize(Roles = "admin")]
    public Task<string> Authorize()
    {
        var user = User.IsInRole("admin");
        return Task.FromResult("123");
    }
    
    /// <summary>
    /// 集成CAP的工作单元
    /// </summary>
    /// <returns></returns>
    [HttpGet("unitofwork"), UnitOfWork(WithCap = true)]
    public async Task<string> UnitOfWork()
    {
        await _capPublisher.PublishAsync<string>("hello", "hello");
        await _mongoRepository.AddOneAsync(new MongoModel() { UserName = "hello" });
        // throw new Exception("ex");
        return "123";
    }

    [NonAction, CapSubscribe("hello")]
    public void Test(string str)
    {
        _logger.LogInformation("接收到了消息" + str);
    }
}