using System.Net.Http.Json;
using DataTorque.Api.Models;

namespace DataTorque.Api.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"]
            ?? throw new InvalidOperationException("OpenWeatherMap API key is not configured.");
    }

    public async Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";

        var owmResponse = await _httpClient.GetFromJsonAsync<OpenWeatherMapResponse>(url)
            ?? throw new HttpRequestException("Failed to get weather data from OpenWeatherMap.");

        var temperatureC = Math.Round(owmResponse.Main.Temp, 1);
        var windSpeedKmh = Math.Round(owmResponse.Wind.Speed * 3.6, 1); // m/s to km/h
        var condition = MapCondition(owmResponse, windSpeedKmh);
        var recommendation = GetRecommendation(temperatureC, condition);

        return new WeatherResponse
        {
            TemperatureCelsius = temperatureC,
            WindSpeedKmh = windSpeedKmh,
            Condition = condition,
            Recommendation = recommendation
        };
    }

    public static string MapCondition(OpenWeatherMapResponse response, double windSpeedKmh)
    {
        var mainWeather = response.Weather.FirstOrDefault()?.Main ?? "";

        return mainWeather.ToLower() switch
        {
            "rain" or "drizzle" or "thunderstorm" => "Rainy",
            "snow" => "Snowing",
            _ => windSpeedKmh > 30 ? "Windy" : "Sunny"
        };
    }

    public static string GetRecommendation(double temperatureC, string condition)
    {
        if (temperatureC > 25)
            return "It's a great day for a swim";

        if (temperatureC < 15 && condition is "Rainy" or "Snowing")
            return "Don't forget to bring a coat";

        if (condition == "Sunny")
            return "Don't forget to bring a hat";

        if (condition == "Rainy")
            return "Don't forget the umbrella";

        return "Enjoy your day!";
    }
}
