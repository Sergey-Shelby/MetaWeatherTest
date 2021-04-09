using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MetaWeatherTest.Models;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace MetaWeatherTest.Weather
{
    public class WeatherClient
    {
        private RestClient _client;
        public WeatherClient()
        {
            _client = new RestClient();
            _client.UseNewtonsoftJson();
        }
        private int GetCityId(Cities city)
        {
            return GetCity(city).Woeid;
        }
        private T Execute<T>(string url) where T : new()
        {
            var response = _client.Execute<T>(new RestRequest(url));

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                Assert.Fail($"{message}: {response.ErrorException.StackTrace}");
            }
            return response.Data;
        }
        private IEnumerable<string> GetCommonWeatherNameStatCity(Cities city, DateTime date)
        {
            var weatherByDate = GetWeatherCity(city, date).Select(x => x.WeatherStateName);
            var weatherNow = GetWeatherCity(city, DateTime.Now).Select(x => x.WeatherStateName);
            return weatherByDate.Intersect(weatherNow);
        }
        public IEnumerable<double> GetMinTemperature(Cities city, DateTime date)
        {
            return GetWeatherCity(city, date).Select(m => m.MinTemp);
        }
        public WeatherCity GetWeatherByCity(Cities city)
        {
            var cityId = GetCityId(city);
            return Execute<WeatherCity>($"https://www.metaweather.com/api/location/{cityId}/");
        }
        public City GetCity(Cities city)
        {
            return Execute<List<City>>($"https://www.metaweather.com/api/location/search/?query={city}")[0];
        }
        public List<WeatherCityDate> GetWeatherCity(Cities city, DateTime date)
        {
            var cityId = GetCityId(city);
            var url = $"https://www.metaweather.com/api/location/{cityId}/{date.Year}/{date.Month}/{date.Day}/";
            return Execute<List<WeatherCityDate>>(url);
        }
        public IEnumerable<double> GetCityLatLong(Cities city)
        {
            return GetCity(city).LattLong.Split(',').Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture));
        }
        public bool IsCommonWeather(Cities city, DateTime date)
        {
            return GetCommonWeatherNameStatCity(city, date).Count<string>() > 0 ? true : false;
        }
        public bool IsAssertSeasonTemperature(WeatherCity weatherCities)
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
    }
}