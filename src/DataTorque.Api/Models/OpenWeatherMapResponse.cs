using System.Text.Json.Serialization;

namespace DataTorque.Api.Models;

public class OpenWeatherMapResponse
{
    [JsonPropertyName("main")]
    public MainData Main { get; set; } = null!;

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; } = null!;

    [JsonPropertyName("weather")]
    public List<WeatherInfo> Weather { get; set; } = [];
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public class WeatherInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Main { get; set; } = null!;
}
