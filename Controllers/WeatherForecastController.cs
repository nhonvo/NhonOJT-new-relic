using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using NewRelic.Api.Agent;

namespace new_relic.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        try
        {
            // Push a custom attribute to New Relic
            NewRelicMonitor.CustomMonitor("Weather", new List<string> { "1", });
            NewRelicMonitor.CustomMonitor("Weather", new List<string> { "2", "Success2" });
            NewRelicMonitor.CustomMonitor("Weather", new List<string> { "3", "Success3", "abc3" });
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        catch (Exception ex)
        {
            // Log the exception as an error in New Relic
            NewRelic.Api.Agent.NewRelic.NoticeError(ex);

            throw; // rethrow the exception to let it propagate to the global exception handler
        }
    }
    [HttpGet("GetWeatherForecast1")]
    [Transaction]
    public IEnumerable<WeatherForecast> Get1()
    {
        try
        {
            // Push a custom attribute to New Relic
            LoggingMiddleware.AddParameter("CustomAttribute", "CustomValue", addToNewRelic: true);
            List<int> a = null;
            a.ForEach(x =>
            {
                System.Console.WriteLine(x);
            });
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        catch (Exception ex)
        {
            // Log the exception as an error in New Relic
            NewRelicMonitor.ErrorCustomMonitor("Error Weather", ex.Message);

            throw; // rethrow the exception to let it propagate to the global exception handler
        }
    }
}
