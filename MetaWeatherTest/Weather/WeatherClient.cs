using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaWeatherTest.Models;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace MetaWeatherTest.Weather
{
    public class WeatherClient
    {
        private readonly RestClient _client;
        private readonly string _baseUrl = "https://www.metaweather.com/api";
        public WeatherClient()
        {
            _client = new RestClient();
            _client.UseNewtonsoftJson();
        }
        public Task<List<WeatherCityDate>> GetWeatherAsync(Cities city, DateTime date)
        {
            var url = $"{_baseUrl}/location/{(int)city}/{date.Year}/{date.Month}/{date.Day}/";
            return ExecuteAsync<List<WeatherCityDate>>(url);
        }
        public Task<WeatherCity> GetWeatherAsync(Cities city)
        {
            return ExecuteAsync<WeatherCity>($"{_baseUrl}/location/{(int)city}/"); 
        }
        public async Task<City> GetCity(Cities city)
        {
			var modelCity = await ExecuteAsync<List<City>>($"{_baseUrl}/location/search/?query={city}");
			return modelCity.FirstOrDefault();
        }
        private async Task<T> ExecuteAsync<T>(string url) where T : new()
        {
            var response = await _client.ExecuteAsync<T>(new RestRequest(url));

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                Assert.Fail($"{message}: {response.ErrorException.StackTrace}");
            }
            return response.Data;
        }
    }
}