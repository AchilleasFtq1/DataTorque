using DataTorque.Api.Models;
using DataTorque.Api.Services;

namespace DataTorque.Api.Tests;

public class RecommendationTests
{
    [Fact]
    public void HotDay_ReturnsSwimRecommendation()
    {
        var result = WeatherService.GetRecommendation(30, "Sunny");
        Assert.Equal("It's a great day for a swim", result);
    }

    [Fact]
    public void HotAndRainy_StillReturnsSwim()
    {
        // over 25 takes priority
        var result = WeatherService.GetRecommendation(26, "Rainy");
        Assert.Equal("It's a great day for a swim", result);
    }

    [Fact]
    public void ColdAndRainy_ReturnsCoatRecommendation()
    {
        var result = WeatherService.GetRecommendation(10, "Rainy");
        Assert.Equal("Don't forget to bring a coat", result);
    }

    [Fact]
    public void ColdAndSnowing_ReturnsCoatRecommendation()
    {
        var result = WeatherService.GetRecommendation(5, "Snowing");
        Assert.Equal("Don't forget to bring a coat", result);
    }

    [Fact]
    public void SunnyDay_ReturnsHatRecommendation()
    {
        var result = WeatherService.GetRecommendation(20, "Sunny");
        Assert.Equal("Don't forget to bring a hat", result);
    }

    [Fact]
    public void RainyButNotCold_ReturnsUmbrellaRecommendation()
    {
        var result = WeatherService.GetRecommendation(18, "Rainy");
        Assert.Equal("Don't forget the umbrella", result);
    }

    [Fact]
    public void WindyDay_ReturnsDefaultRecommendation()
    {
        var result = WeatherService.GetRecommendation(20, "Windy");
        Assert.Equal("Enjoy your day!", result);
    }

    [Fact]
    public void ExactlyFifteenAndRaining_ReturnsUmbrella()
    {
        // 15 is NOT less than 15, so coat rule doesn't apply
        var result = WeatherService.GetRecommendation(15, "Rainy");
        Assert.Equal("Don't forget the umbrella", result);
    }

    [Fact]
    public void ExactlyTwentyFive_ReturnsHat()
    {
        // 25 is NOT over 25, so swim rule doesn't apply
        var result = WeatherService.GetRecommendation(25, "Sunny");
        Assert.Equal("Don't forget to bring a hat", result);
    }
}

public class ConditionMappingTests
{
    [Fact]
    public void ClearWeather_LowWind_ReturnsSunny()
    {
        var response = MakeOwmResponse("Clear", 5);
        var result = WeatherService.MapCondition(response, 5 * 3.6); // 18 km/h - under 30
        Assert.Equal("Sunny", result);
    }

    [Fact]
    public void ClearWeather_HighWind_ReturnsWindy()
    {
        var response = MakeOwmResponse("Clear", 10);
        var result = WeatherService.MapCondition(response, 35);
        Assert.Equal("Windy", result);
    }

    [Fact]
    public void RainWeather_ReturnsRainy()
    {
        var response = MakeOwmResponse("Rain", 5);
        var result = WeatherService.MapCondition(response, 5 * 3.6);
        Assert.Equal("Rainy", result);
    }

    [Fact]
    public void DrizzleWeather_ReturnsRainy()
    {
        var response = MakeOwmResponse("Drizzle", 3);
        var result = WeatherService.MapCondition(response, 3 * 3.6);
        Assert.Equal("Rainy", result);
    }

    [Fact]
    public void ThunderstormWeather_ReturnsRainy()
    {
        var response = MakeOwmResponse("Thunderstorm", 8);
        var result = WeatherService.MapCondition(response, 8 * 3.6);
        Assert.Equal("Rainy", result);
    }

    [Fact]
    public void SnowWeather_ReturnsSnowing()
    {
        var response = MakeOwmResponse("Snow", 4);
        var result = WeatherService.MapCondition(response, 4 * 3.6);
        Assert.Equal("Snowing", result);
    }

    [Fact]
    public void CloudyWeather_LowWind_ReturnsSunny()
    {
        var response = MakeOwmResponse("Clouds", 5);
        var result = WeatherService.MapCondition(response, 5 * 3.6);
        Assert.Equal("Sunny", result);
    }

    private static OpenWeatherMapResponse MakeOwmResponse(string mainWeather, double windSpeed)
    {
        return new OpenWeatherMapResponse
        {
            Main = new MainData { Temp = 20 },
            Wind = new WindData { Speed = windSpeed },
            Weather = new List<WeatherInfo>
            {
                new() { Id = 800, Main = mainWeather }
            }
        };
    }
}

public class RequestCounterTests
{
    [Fact]
    public void EveryFifthRequest_IsMultipleOfFive()
    {
        var counter = new RequestCounter();

        for (int i = 1; i <= 20; i++)
        {
            var count = counter.Increment();
            Assert.Equal(i, count);
        }
    }

    [Fact]
    public void FifthRequest_ShouldTrigger503()
    {
        var counter = new RequestCounter();

        for (int i = 1; i <= 4; i++)
            counter.Increment();

        var fifth = counter.Increment();
        Assert.Equal(0, fifth % 5);
    }

    [Fact]
    public void TenthRequest_ShouldAlsoTrigger503()
    {
        var counter = new RequestCounter();

        for (int i = 1; i <= 9; i++)
            counter.Increment();

        var tenth = counter.Increment();
        Assert.Equal(0, tenth % 5);
    }
}
