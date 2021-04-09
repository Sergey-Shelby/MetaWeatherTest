using MetaWeatherTest.Weather;
using NUnit.Framework;
using System;
using System.Linq;

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

        [Test]
        public void MinimumTemperatureInCityTest()
        {
            var actualMinTemperature = _client.GetMinTemperature(Cities.Minsk, new DateTime(2021, 1, 1));
            var expectedMinTemperature = -0.235;
            Assert.That(actualMinTemperature, Does.Contain(expectedMinTemperature), "Minimum temperature comparison error");
        }

        [Test]
        public void LatitudeLongitudeOfCityTest()
        {
            var lattLongCity = _client.GetCityLatLong(Cities.Minsk);
            var actualLatitude = lattLongCity.ElementAt(0);
            var actualLongitude = lattLongCity.ElementAt(1);
            var expectedLatitude = 53.9;
            var expectedLongitude = 27.5;
            Assert.Multiple(() =>
            {
                Assert.That(actualLatitude, Is.EqualTo(expectedLatitude).Within(0.095), "Latitude does not match");
                Assert.That(actualLongitude, Is.EqualTo(expectedLongitude).Within(0.095), "Longitude does not match");
            });
        }

        [Test]
        public void ActualWeatherCityTest()
        {
            var actualWeatherCityDate = _client.GetWeatherCity(Cities.Minsk, DateTime.Now).Select(x => x.ApplicableDate);
            var expectedDate = DateTime.Now.ToString("yyyy-MM-dd");
            CollectionAssert.Contains(actualWeatherCityDate, expectedDate);
        }

        [Test]
        public void TemperatureSeasonInRangeTest()
        {
            var weatherCity = _client.GetWeatherByCity(Cities.Minsk);
            var actualIsSeasonTemperature = _client.IsAssertSeasonTemperature(weatherCity);
            Assert.That(actualIsSeasonTemperature, "Temperature is out of range");
        }

        [Test]
        public void WeatherStateByDateTest()
        {
            var date = DateTime.Now.AddYears(-5);
            var actualIsCommonWeather = _client.IsCommonWeather(Cities.London, date);
            Assert.That(actualIsCommonWeather, "No common values");
        }
    }
}