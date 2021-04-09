using MetaWeatherTest.Models;
using MetaWeatherTest.Weather;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetaWeatherTest
{
    public class Tests
    {
        private WeatherClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new WeatherClient();
        }

        private bool IsAssertSeasonTemperature(WeatherCity weatherCities)
        {
            foreach (var item in weatherCities.ConsolidatedWeather)
            {
                Seasons season = item.ApplicableDate.Season();
                switch (season)
                {
                    case Seasons.Winter: if (item.TheTemp < -50 && item.TheTemp > 10) return false; break;
                    case Seasons.Spring: if (item.TheTemp < -10 && item.TheTemp > 30) return false; break;
                    case Seasons.Summer: if (item.TheTemp < 0 && item.TheTemp > 40) return false; break;
                    case Seasons.Autumn: if (item.TheTemp < -10 && item.TheTemp > 20) return false; break;
                    default: return false;
                }
            }
            return true;
        }
        [Test]
        public async Task MinimumTemperatureInCityTest()
        {
            var actualMinTemperature = await _client.GetMinTemperature(Cities.Minsk, new DateTime(2021, 1, 1));
            var expectedMinTemperature = -0.235;
            Assert.That(actualMinTemperature, Does.Contain(expectedMinTemperature), "Minimum temperature comparison error");
        }

        [Test]
        public async Task LatitudeLongitudeOfCityTest()
        {
            var latlong = await _client.GetCityLatLong(Cities.Minsk);
            var expectedLatitude = 53.9;
            var expectedLongitude = 27.5;
            Assert.Multiple(() =>
            {
                Assert.That(latlong.Latitude, Is.EqualTo(expectedLatitude).Within(0.099), "Latitude does not match");
                Assert.That(latlong.Longitude, Is.EqualTo(expectedLongitude).Within(0.099), "Longitude does not match");
            });
        }

        [Test]
        public async Task ActualWeatherCityTest()
        {
            var listActualWeatherCityDate = await _client.GetWeatherAsync(Cities.Minsk, DateTime.Now);
            var actualWeatherCityDate = listActualWeatherCityDate.Select(x => x.ApplicableDate);
            var expectedDate = DateTime.Now.ToString("yyyy-MM-dd");
            CollectionAssert.Contains(actualWeatherCityDate, expectedDate);
        }

        [Test]
        public async Task TemperatureSeasonInRangeTest()
        {
            var weatherCity = await _client.GetWeatherAsync(Cities.Minsk);
            var actualIsSeasonTemperature = IsAssertSeasonTemperature(weatherCity);
            Assert.That(actualIsSeasonTemperature, "Temperature is out of range");
        }

        [Test]
        public async Task WeatherStateByDateTest()
        {
            var date = DateTime.Now.AddYears(-5);
            var listWeatherByDate = await _client.GetWeatherAsync(Cities.London, date);
            var weatherByDate = listWeatherByDate.Select(x => x.WeatherStateName);
            var listWeatherNow = await _client.GetWeatherAsync(Cities.London, DateTime.Now);
            var weatherNow = listWeatherNow.Select(x => x.WeatherStateName);
            Assert.That(weatherByDate.Intersect(weatherNow).Any(), Is.True, "Not contains same state");
        }
    }
}