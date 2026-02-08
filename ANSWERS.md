# Answers

## 3. Assumptions

- The recommendation rules can overlap (e.g. sunny + over 25), so I picked a priority: hot weather first, then cold+wet, then sunny, then rainy. If it's 30 degrees I don't care if it's sunny, you should go swim.

- OpenWeatherMap has heaps of weather types (Drizzle, Thunderstorm, Mist, etc) but the spec only wanted four. I lumped Drizzle and Thunderstorm in with Rain. Anything that's not rain or snow is either Sunny or Windy depending on wind speed. I used 30 km/h as the windy threshold — seemed about right for Wellington.

- The 503 counter is global, not per-user. Just a static int. Request 5, 10, 15 etc. get a 503.

- "Over 25" means strictly > 25. "Less than 15" means strictly < 15. So 25.0 gets a hat, 15.0 gets an umbrella.

- OWM gives wind in m/s, converted to km/h with * 3.6.

- Frontend uses browser geolocation to grab your location automatically, falls back to Wellington if you deny it.

## 4. Improvements

- Caching — even just a couple minutes of IMemoryCache per coordinate would cut down on OWM calls heaps. Weather doesn't change that fast.

- Proper input validation on lat/lon. Right now you could pass 999 and it'd just forward garbage to OWM. Should return 400 for out of range values.

- Polly retry/circuit-breaker on the HTTP client. If OWM is having a bad day we just 502 straight away, which isn't great.

- Move the API key to user-secrets instead of sitting in appsettings.json.

- Frontend could use a loading skeleton, a retry button on errors, and ideally some React Testing Library tests. A map picker for coords would be nicer than typing numbers.

- Docker compose to run everything with one command.
