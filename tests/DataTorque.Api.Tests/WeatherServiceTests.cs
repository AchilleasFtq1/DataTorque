using DataTorque.Api.Models;
using DataTorque.Api.Services;

namespace DataTorque.Api.Tests;

public class RecommendationTests
{
    [Fact]
    public void HotDay_Swim()
    {
        Assert.Equal("It's a great day for a swim", WeatherService.GetRecommendation(30, "Sunny"));
    }

    [Fact]
    public void HotDay_TrumpsRain()
    {
        Assert.Equal("It's a great day for a swim", WeatherService.GetRecommendation(26, "Rainy"));
    }

    [Fact]
    public void ColdRain_Coat()
    {
        Assert.Equal("Don't forget to bring a coat", WeatherService.GetRecommendation(10, "Rainy"));
    }

    [Fact]
    public void ColdSnow_Coat()
    {
        Assert.Equal("Don't forget to bring a coat", WeatherService.GetRecommendation(5, "Snowing"));
    }

    [Fact]
    public void Sunny_Hat()
    {
        Assert.Equal("Don't forget to bring a hat", WeatherService.GetRecommendation(20, "Sunny"));
    }

    [Fact]
    public void MildRain_Umbrella()
    {
        Assert.Equal("Don't forget the umbrella", WeatherService.GetRecommendation(18, "Rainy"));
    }

    [Fact]
    public void Windy_DefaultMsg()
    {
        Assert.Equal("Enjoy your day!", WeatherService.GetRecommendation(20, "Windy"));
    }

    [Fact]
    public void BoundaryAt15_NotCold()
    {
        // 15 is not < 15, so no coat
        Assert.Equal("Don't forget the umbrella", WeatherService.GetRecommendation(15, "Rainy"));
    }

    [Fact]
    public void BoundaryAt25_NotHot()
    {
        // 25 is not > 25, so no swim
        Assert.Equal("Don't forget to bring a hat", WeatherService.GetRecommendation(25, "Sunny"));
    }
}

public class ConditionMappingTests
{
    [Theory]
    [InlineData("Rain", "Rainy")]
    [InlineData("Drizzle", "Rainy")]
    [InlineData("Thunderstorm", "Rainy")]
    [InlineData("Snow", "Snowing")]
    public void WetWeatherTypes(string owmMain, string expected)
    {
        var resp = BuildResponse(owmMain, 5);
        Assert.Equal(expected, WeatherService.MapCondition(resp, 5 * 3.6));
    }

    [Fact]
    public void Clear_LowWind_Sunny()
    {
        var resp = BuildResponse("Clear", 5);
        Assert.Equal("Sunny", WeatherService.MapCondition(resp, 18));
    }

    [Fact]
    public void Clear_HighWind_Windy()
    {
        var resp = BuildResponse("Clear", 10);
        Assert.Equal("Windy", WeatherService.MapCondition(resp, 35));
    }

    [Fact]
    public void Clouds_TreatedAsSunny()
    {
        var resp = BuildResponse("Clouds", 3);
        Assert.Equal("Sunny", WeatherService.MapCondition(resp, 10));
    }

    private static OpenWeatherMapResponse BuildResponse(string weatherMain, double windSpeed) => new()
    {
        Main = new MainData { Temp = 20 },
        Wind = new WindData { Speed = windSpeed },
        Weather = [new() { Id = 800, Main = weatherMain }]
    };
}
