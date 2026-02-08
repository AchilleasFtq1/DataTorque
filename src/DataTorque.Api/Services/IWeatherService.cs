using DataTorque.Api.Models;

namespace DataTorque.Api.Services;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude);
}
