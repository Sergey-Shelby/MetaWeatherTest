using MetaWeatherTest.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MetaWeatherTest.Weather
{
	class LatLongConverter : JsonConverter<LatLong>
	{
		public override LatLong ReadJson(JsonReader reader, Type objectType, [AllowNull] LatLong existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var listLatLong = reader.Value.ToString().Split(',').Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture));
			if (listLatLong.Count() != 2)
			{
				throw new ArithmeticException($"Invalid format: {reader.Value}");
			}
			return new LatLong { Latitude = listLatLong.FirstOrDefault(), Longitude = listLatLong.LastOrDefault() };
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] LatLong value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
