using System;
using System.Globalization;
using System.Linq;

namespace MetaWeatherTest.Models
{
	public class LatLong
	{
		public double Latitude { get; }
		public double Longitude { get; }
		public LatLong(City city)
		{
			var listLatLong = city.LattLong.Split(',').Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture));
			this.Latitude = listLatLong.ElementAt(0);
			this.Longitude = listLatLong.ElementAt(1);
		}
	}
}
