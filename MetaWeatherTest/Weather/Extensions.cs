using System;

namespace MetaWeatherTest.Weather
{
    public static class Extensions
    {
        public static Seasons Season(this string dateString)
        {
            DateTime date = DateTime.Parse(dateString);
            int doy = date.DayOfYear - Convert.ToInt32((DateTime.IsLeapYear(date.Year)) && date.DayOfYear > 59);

            if (doy < 59 || doy >= 334) return Seasons.Winter;

            if (doy >= 59 && doy < 151) return Seasons.Spring;

            if (doy >= 151 && doy < 244) return Seasons.Summer;

            return Seasons.Autumn;
        }
    }
}