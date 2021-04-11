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

        [Test]
        public async Task MinimumTemperatureInCityTest()
        {
            var listWeather = await _client.GetWeatherAsync(Cities.Minsk, new DateTime(2021, 1, 1));
            var actualMinTemperature = listWeather.Select(m => m.MinTemp);
            var expectedMinTemperature = -0.235;
            Assert.That(actualMinTemperature, Does.Contain(expectedMinTemperature), "Minimum temperature comparison error");
        }

        [Test]
        public async Task LatitudeLongitudeOfCityTest()
        {
            var city = await _client.GetCity(Cities.Minsk);
            var expectedLatitude = 53.9;
            var expectedLongitude = 27.5;
            Assert.Multiple(() =>
            {
                Assert.That(city.LattLong.Latitude, Is.EqualTo(expectedLatitude).Within(0.099), "Latitude does not match");
                Assert.That(city.LattLong.Longitude, Is.EqualTo(expectedLongitude).Within(0.099), "Longitude does not match");
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
            AssertSeasonTemperature(weatherCity);
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

        private void AssertSeasonTemperature(WeatherCity weatherCities)
        {
            foreach (var day in weatherCities.ConsolidatedWeather)
            {
                Seasons season = day.ApplicableDate.Season();
                switch (season)
                {
                    case Seasons.Winter:
                        Assert.That(day.TheTemp, Is.InRange(-50, 10), $"Weather for {season} (date: {day.ApplicableDate})");
                        break;
                    case Seasons.Spring:
                        Assert.That(day.TheTemp, Is.InRange(-10, 30), $"Weather for {season} (date: {day.ApplicableDate})");
                        break;
                    case Seasons.Summer:
                        Assert.That(day.TheTemp, Is.InRange(0, 40), $"Weather for {season} (date: {day.ApplicableDate})");
                        break;
                    case Seasons.Autumn:
                        Assert.That(day.TheTemp, Is.InRange(-10, 20), $"Weather for {season} (date: {day.ApplicableDate})");
                        break;
                }
            }
        }
    }
}