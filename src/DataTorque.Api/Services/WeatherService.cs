using System.Net.Http.Json;
using DataTorque.Api.Models;

namespace DataTorque.Api.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["OpenWeatherMap:ApiKey"]
            ?? throw new InvalidOperationException("Missing OpenWeatherMap API key in config.");
    }

    public async Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";

        var data = await _httpClient.GetFromJsonAsync<OpenWeatherMapResponse>(url)
            ?? throw new HttpRequestException("Got null back from OpenWeatherMap");

        var tempC = Math.Round(data.Main.Temp, 1);
        var windKmh = Math.Round(data.Wind.Speed * 3.6, 1);
        var condition = MapCondition(data, windKmh);

        return new WeatherResponse
        {
            TemperatureCelsius = tempC,
            WindSpeedKmh = windKmh,
            Condition = condition,
            Recommendation = GetRecommendation(tempC, condition)
        };
    }

    public static string MapCondition(OpenWeatherMapResponse data, double windKmh)
    {
        var main = data.Weather.FirstOrDefault()?.Main ?? "";

        return main.ToLower() switch
        {
            "rain" or "drizzle" or "thunderstorm" => "Rainy",
            "snow" => "Snowing",
            _ => windKmh > 30 ? "Windy" : "Sunny"
        };
    }

    public static string GetRecommendation(double tempC, string condition)
    {
        if (tempC > 25)
            return "It's a great day for a swim";

        if (tempC < 15 && condition is "Rainy" or "Snowing")
            return "Don't forget to bring a coat";

        if (condition == "Sunny")
            return "Don't forget to bring a hat";

        if (condition == "Rainy")
            return "Don't forget the umbrella";

        return "Enjoy your day!";
    }
}
