import { useEffect, useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

// in dev vite proxies to the API, in prod they're on the same origin
const API_URL = ""

// wellington as fallback if geolocation fails
const DEFAULT_LAT = "-41.2865"
const DEFAULT_LNG = "174.7762"

interface WeatherData {
  temperatureCelsius: number
  windSpeedKmh: number
  condition: string
  recommendation: string
}

const weatherIcons: Record<string, string> = {
  Sunny: "☀️",
  Rainy: "🌧️",
  Snowing: "❄️",
  Windy: "💨",
}

function App() {
  const [lat, setLat] = useState("")
  const [lng, setLng] = useState("")
  const [weather, setWeather] = useState<WeatherData | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [located, setLocated] = useState(false)

  // try to grab the user's location on first load
  useEffect(() => {
    if (!navigator.geolocation) {
      setLat(DEFAULT_LAT)
      setLng(DEFAULT_LNG)
      setLocated(true)
      return
    }

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setLat(pos.coords.latitude.toFixed(4))
        setLng(pos.coords.longitude.toFixed(4))
        setLocated(true)
      },
      () => {
        // user denied or something went wrong, fall back to wellington
        setLat(DEFAULT_LAT)
        setLng(DEFAULT_LNG)
        setLocated(true)
      }
    )
  }, [])

  // auto-fetch once we have coordinates
  useEffect(() => {
    if (located && lat && lng) {
      fetchWeather()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [located])

  async function fetchWeather() {
    setLoading(true)
    setError("")
    setWeather(null)

    try {
      const res = await fetch(`${API_URL}/weather?latitude=${lat}&longitude=${lng}`)

      if (res.status === 503) {
        setError("Service temporarily unavailable — try again in a sec.")
        return
      }
      if (!res.ok) {
        setError(`Something went wrong (${res.status})`)
        return
      }

      const data: WeatherData = await res.json()
      setWeather(data)
    } catch {
      setError("Can't reach the API. Is the backend running?")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-4">
      <div className="w-full max-w-md space-y-4">
        <div className="text-center space-y-1">
          <h1 className="text-2xl font-bold tracking-tight">Wellington Weather</h1>
          <p className="text-sm text-muted-foreground">
            What to wear before heading out
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Location</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label htmlFor="lat">Latitude</Label>
                <Input
                  id="lat"
                  value={lat}
                  onChange={(e) => setLat(e.target.value)}
                  placeholder="-41.2865"
                />
              </div>
              <div className="space-y-1">
                <Label htmlFor="lng">Longitude</Label>
                <Input
                  id="lng"
                  value={lng}
                  onChange={(e) => setLng(e.target.value)}
                  placeholder="174.7762"
                />
              </div>
            </div>
            <Button onClick={fetchWeather} disabled={loading} className="w-full">
              {loading ? "Checking..." : "Check Weather"}
            </Button>
          </CardContent>
        </Card>

        {error && (
          <Card className="border-destructive">
            <CardContent className="pt-4">
              <p className="text-sm text-destructive">{error}</p>
            </CardContent>
          </Card>
        )}

        {weather && (
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="text-center">
                <span className="text-5xl">
                  {weatherIcons[weather.condition] || "🌤️"}
                </span>
                <p className="text-lg font-medium mt-2">{weather.condition}</p>
              </div>

              <div className="grid grid-cols-2 gap-4 text-center">
                <div>
                  <p className="text-2xl font-bold">{weather.temperatureCelsius}°C</p>
                  <p className="text-xs text-muted-foreground">Temperature</p>
                </div>
                <div>
                  <p className="text-2xl font-bold">{weather.windSpeedKmh} km/h</p>
                  <p className="text-xs text-muted-foreground">Wind</p>
                </div>
              </div>

              <div className="bg-muted rounded-lg p-3 text-center">
                <p className="text-sm font-medium">{weather.recommendation}</p>
              </div>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  )
}

export default App
