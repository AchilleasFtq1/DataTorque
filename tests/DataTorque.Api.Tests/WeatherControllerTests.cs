using DataTorque.Api.Models;
using DataTorque.Api.Services;
using DataTorque.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DataTorque.Api.Tests;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;

    public WeatherControllerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _weatherServiceMock.Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new WeatherResponse
            {
                TemperatureCelsius = 20,
                WindSpeedKmh = 10,
                Condition = "Sunny",
                Recommendation = "Don't forget to bring a hat"
            });
    }

    [Fact]
    public async Task Get_NormalRequest_Returns200()
    {
        var counter = new RequestCounter();
        var controller = new WeatherController(_weatherServiceMock.Object, counter);

        var result = await controller.Get(-41.2865, 174.7762);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var weather = Assert.IsType<WeatherResponse>(okResult.Value);
        Assert.Equal(20, weather.TemperatureCelsius);
        Assert.Equal("Sunny", weather.Condition);
    }

    [Fact]
    public async Task Get_FifthRequest_Returns503()
    {
        var counter = new RequestCounter();
        var controller = new WeatherController(_weatherServiceMock.Object, counter);

        // burn through 4 requests
        for (int i = 0; i < 4; i++)
            await controller.Get(-41.2865, 174.7762);

        // 5th should be 503
        var result = await controller.Get(-41.2865, 174.7762);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, statusResult.StatusCode);
    }

    [Fact]
    public async Task Get_TenthRequest_AlsoReturns503()
    {
        var counter = new RequestCounter();
        var controller = new WeatherController(_weatherServiceMock.Object, counter);

        // burn through 9 requests
        for (int i = 0; i < 9; i++)
            await controller.Get(-41.2865, 174.7762);

        // 10th should also be 503
        var result = await controller.Get(-41.2865, 174.7762);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, statusResult.StatusCode);
    }

    [Fact]
    public async Task Get_SixthRequest_Returns200()
    {
        var counter = new RequestCounter();
        var controller = new WeatherController(_weatherServiceMock.Object, counter);

        // burn through 5 requests (5th is 503)
        for (int i = 0; i < 5; i++)
            await controller.Get(-41.2865, 174.7762);

        // 6th should be 200
        var result = await controller.Get(-41.2865, 174.7762);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Get_WhenServiceThrows_Returns502()
    {
        var failingService = new Mock<IWeatherService>();
        failingService.Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ThrowsAsync(new HttpRequestException("upstream down"));

        var counter = new RequestCounter();
        var controller = new WeatherController(failingService.Object, counter);

        var result = await controller.Get(-41.2865, 174.7762);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(502, statusResult.StatusCode);
    }
}
