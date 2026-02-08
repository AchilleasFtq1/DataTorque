using DataTorque.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataTorque.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IRequestCounter _requestCounter;

    public WeatherController(IWeatherService weatherService, IRequestCounter requestCounter)
    {
        _weatherService = weatherService;
        _requestCounter = requestCounter;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] double latitude, [FromQuery] double longitude)
    {
        var count = _requestCounter.Increment();

        if (count % 5 == 0)
        {
            return StatusCode(503, new { error = "Service temporarily unavailable. Please try again." });
        }

        try
        {
            var weather = await _weatherService.GetWeatherAsync(latitude, longitude);
            return Ok(weather);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { error = "Unable to fetch weather data from upstream provider." });
        }
    }
}
