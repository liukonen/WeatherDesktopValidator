using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WeatherDesktopSharedCore.Interfaces
{
   public class WeatherAPI3 : BaseWeather
    {

        private string _exception = string.Empty;
        private string _forcastDescription = string.Empty;
        private int _temp = 0;
        private Boolean _Success = false;
        private WeatherTypes _type;

        public override string ErrorMessages()
        {
            return _exception;
        }

        public override string ForcastDescription()
        {
            return _forcastDescription;
        }

        const string Gov_User = "WeatherWallpaper/v1.0 (https://github.com/liukonen/WeatherWallpaper/; liukonen@gmail.com)";

        public override void Invoke()
        {

            try {
                string S = CompressedCallSite("https://api.weather.gov/gridpoints/MKX/80,70/forecast/hourly", Gov_User);
                JObject item = JObject.Parse(S);
                var X = item["properties"]["periods"][0];
                _temp = X.Value<int>("temperature");
                _forcastDescription = X.Value<string>("shortForecast");
                _Success = true;
                _type = convert(_forcastDescription);
            }

            catch (Exception x) { _exception = x.ToString(); }
        }

        private WeatherTypes convert(string description)
        {
            if (description.ToLower().Contains("thunderstorm")) { return WeatherTypes.ThunderStorm; }
            if (description.ToLower().Contains("partly cloudy")) { return WeatherTypes.PartlyCloudy; }
            if (description.ToLower().Contains("snow")) { return WeatherTypes.Snow; }
            if (description.ToLower().Contains("cloudy")) { return WeatherTypes.Cloudy; }
            if (description.ToLower().Contains("rain")) { return WeatherTypes.Rain; }
            if (description.ToLower().Contains("dust")) { return WeatherTypes.Dust; }
            if (description.ToLower().Contains("Fog")) { return WeatherTypes.Fog; }
            if (description.ToLower().Contains("frigid")) { return WeatherTypes.Frigid; }
            if (description.ToLower().Contains("haze")) { return WeatherTypes.Haze; }
            if (description.ToLower().Contains("hot")) { return WeatherTypes.Hot; }
            if (description.ToLower().Contains("smoke")) { return WeatherTypes.Smoke; }
            if (description.ToLower().Contains("windy")) { return WeatherTypes.Windy; }
            return WeatherTypes.Clear;
        }

        public override string PostUrl()
        {
            return "https://api.weather.gov/gridpoints/MKX/80,70/forecast/hourly";
        }

        public override bool Success()
        {
            return _Success;
        }

        public override int Temp()
        {
            return _temp;
        }

        public override WeatherTypes WType()
        {
            return _type;
        }
    }
}
