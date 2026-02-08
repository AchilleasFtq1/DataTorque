using DataTorque.Api.Models;
using DataTorque.Api.Services;
using DataTorque.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DataTorque.Api.Tests;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _mockService;

    public WeatherControllerTests()
    {
        _mockService = new Mock<IWeatherService>();
        _mockService.Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new WeatherResponse
            {
                TemperatureCelsius = 20,
                WindSpeedKmh = 10,
                Condition = "Sunny",
                Recommendation = "Don't forget to bring a hat"
            });
    }

    [Fact]
    public async Task Get_ReturnsWeather()
    {
        var controller = new WeatherController(_mockService.Object);
        var result = await controller.Get(-41.2865, 174.7762);

        // static counter is shared across tests so we might land on a 503
        while (result is ObjectResult { StatusCode: 503 })
            result = await controller.Get(-41.2865, 174.7762);

        var ok = Assert.IsType<OkObjectResult>(result);
        var weather = Assert.IsType<WeatherResponse>(ok.Value);
        Assert.Equal("Sunny", weather.Condition);
    }

    [Fact]
    public async Task Get_503OnEveryFifth()
    {
        var controller = new WeatherController(_mockService.Object);

        // in any window of 5 we must hit at least one 503
        var results = new List<IActionResult>();
        for (int i = 0; i < 5; i++)
            results.Add(await controller.Get(-41.2865, 174.7762));

        Assert.Contains(results, r => r is ObjectResult { StatusCode: 503 });
    }

    [Fact]
    public async Task Get_4OutOf5AreOk()
    {
        var controller = new WeatherController(_mockService.Object);

        var results = new List<IActionResult>();
        for (int i = 0; i < 5; i++)
            results.Add(await controller.Get(-41.2865, 174.7762));

        Assert.Equal(4, results.Count(r => r is OkObjectResult));
    }

    [Fact]
    public async Task Get_UpstreamDown_502()
    {
        var broken = new Mock<IWeatherService>();
        broken.Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ThrowsAsync(new HttpRequestException("nope"));

        var controller = new WeatherController(broken.Object);
        var result = await controller.Get(-41.2865, 174.7762);

        while (result is ObjectResult { StatusCode: 503 })
            result = await controller.Get(-41.2865, 174.7762);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(502, status.StatusCode);
    }
}
