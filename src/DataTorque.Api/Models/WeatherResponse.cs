namespace DataTorque.Api.Models;

public class WeatherResponse
{
    public double TemperatureCelsius { get; set; }
    public double WindSpeedKmh { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}
