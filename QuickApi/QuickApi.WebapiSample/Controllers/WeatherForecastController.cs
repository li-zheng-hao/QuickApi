using Microsoft.AspNetCore.Mvc;
using QuickApi.HttpResponse;

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

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
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
    [HttpGet("get3")]
    public async Task<string> Get3()
    {
        await Task.Yield();
        return "123";
    }
}