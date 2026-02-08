using DataTorque.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// allow the react frontend to call us
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

// fallback to index.html for SPA routing (production)
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
