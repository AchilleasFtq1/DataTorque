using DataTorque.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataTorque.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private static int _requestCount;
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] double latitude, [FromQuery] double longitude)
    {
        // simulate upstream failure every 5th request
        var count = Interlocked.Increment(ref _requestCount);
        if (count % 5 == 0)
            return StatusCode(503, new { error = "Service temporarily unavailable. Please try again." });

        try
        {
            var result = await _weatherService.GetWeatherAsync(latitude, longitude);
            return Ok(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { error = "Failed to reach weather provider." });
        }
    }
}
