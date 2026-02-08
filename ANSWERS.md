# Answers

## 3. Assumptions

- **Recommendation priority order**: When multiple recommendation rules could apply (e.g. it's sunny AND over 25°C), I assumed a priority order: temperature > 25°C takes highest priority (swim), then cold + rain/snow (coat), then sunny (hat), then rain (umbrella). This felt like the most sensible reading — if it's boiling hot, "go for a swim" matters more than "bring a hat".

- **Weather condition mapping**: OpenWeatherMap returns detailed weather categories (Clear, Clouds, Drizzle, Thunderstorm, etc.). I mapped these to the four required conditions: Drizzle and Thunderstorm both count as "Rainy". Clouds and Clear both count as "Sunny" unless wind speed is high. "Windy" is triggered when wind exceeds 30 km/h and it's not already raining or snowing.

- **"Every fifth request" means globally**: The 503 counter is application-wide (singleton), not per-user or per-session. Request 5, 10, 15 etc. get a 503 regardless of who's calling.

- **Wind speed conversion**: OpenWeatherMap returns wind in m/s, so I convert to km/h by multiplying by 3.6.

- **Default recommendation**: For conditions that don't match any specific rule (e.g. windy but mild), I return "Enjoy your day!" as a fallback since the spec didn't cover every combination.

- **Boundary conditions**: "over 25°C" means strictly greater than 25 (25.0 doesn't trigger swim). "less than 15°C" means strictly less than 15 (15.0 doesn't trigger coat).

- **Free tier API key**: The user needs to provide their own OpenWeatherMap API key in appsettings.json. I didn't hardcode one.

## 4. Improvements With More Time

- **Caching**: Add a short-lived cache (like `IMemoryCache`) for weather responses so we're not hitting OpenWeatherMap on every single request. Weather doesn't change that fast. Even 2-3 minutes of caching per lat/lon pair would help.

- **Retry policy with Polly**: Right now if OpenWeatherMap is slow or flaky, we just fail. I'd add Polly for retry/circuit-breaker on the HttpClient so transient upstream failures get handled gracefully.

- **Input validation**: The controller accepts any double for lat/lon. I'd add proper validation — latitude should be -90 to 90, longitude -180 to 180. Return 400 Bad Request with a clear message if they're out of range.

- **Structured error responses**: Currently errors return anonymous objects. I'd create a proper `ErrorResponse` model so the API is consistent in what it returns on failure.

- **Integration tests with WebApplicationFactory**: I wrote unit tests for the controller and service logic, but with more time I'd add proper integration tests using `WebApplicationFactory<Program>` that spin up the full pipeline and test actual HTTP requests end-to-end, mocking just the OpenWeatherMap calls.

- **API key via user secrets**: Instead of appsettings.json, use `dotnet user-secrets` for the API key in development. Cleaner and no risk of accidentally committing it.

- **Frontend improvements**: Add a loading skeleton, better error handling with retry button, maybe a map picker for lat/lon instead of raw number inputs. Also would add some tests with React Testing Library.

- **Docker support**: Add a Dockerfile and docker-compose so the whole thing (API + frontend) can be spun up with one command.

- **Rate limiting**: Beyond the simulated 503, add real rate limiting middleware to protect against abuse.
