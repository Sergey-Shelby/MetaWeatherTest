using MetaWeatherTest.Weather;
using Newtonsoft.Json;

namespace MetaWeatherTest.Models 
{
    public class City
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("woeid")]
        public int Woeid { get; set; }

        [JsonProperty("latt_long")]
        [JsonConverter(typeof(LatLongConverter))]
        public LatLong LattLong { get; set; }
    }
}