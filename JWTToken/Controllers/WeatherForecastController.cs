using JWTToken.Model;
using JWTToken.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTToken.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly GetToken token;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,GetToken token)
    {
        _logger = logger;
        this.token = token;
       
        Console.WriteLine(Guid.NewGuid().ToString());
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            guid = Guid.NewGuid().ToString()
        })
        .ToArray();
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginDetails login)
    {
        if(string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
        {
            return BadRequest("Invalid client request");
        }
        var users=token.GetAllUsers().ToList();
        var valid=users.FirstOrDefault(u => u.UserName == login.UserName && u.Password == login.Password);
        if(valid is null)
        {
            return StatusCode(401, "Unauthorized");
        }
        var jwtToken = token.GenerateToken();
        return Ok(new {User=login.UserName, Token = jwtToken });
    }
}
