using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WeatherDesktopSharedCore.Interfaces
{
    public class MSWeather : BaseWeather
    {
        const string Main = "http://weather.service.msn.com/data.aspx?weasearchstr={0}&culture=en-US&weadegreetype=F&src=outlook";
        private Boolean _Success;
        private int _Temp;
        private string _err;
        private string _forcast;
        private WeatherTypes _type;
        public override string ToString()
        {
            return "MS Weather";
        }

        public override string ErrorMessages() { return _err; }
        public override string ForcastDescription() { return _forcast; }
        public override bool Success() { return _Success; }
        public override int Temp() { return _Temp; }
        public override WeatherTypes WType() { return _type; }

        public override void Invoke()
        {
            try
            {
                string Response = CompressedCallSite(string.Format(Main, ZipCode));
                TransformWeather(Response);
                _Success = true;

            }
            catch (Exception X) { _Success = false; _err = X.ToString(); }

        }

        private void TransformWeather(string webresponse)
        {
            const string forcastFormat = "{3}, {2}. [{1}-{0}] Precipitation {4}%.";
            //WeatherResponse response = new WeatherResponse();
            System.Text.StringBuilder forcast = new System.Text.StringBuilder();

            XmlReader reader = XmlReader.Create(new System.IO.StringReader(webresponse));
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element))
                    switch (reader.Name)
                    {
                        case "current":
                            _Temp = int.Parse(reader.GetAttribute("temperature"));
                            //_skycode = int.Parse(reader.GetAttribute("skycode"));
                            string skyTxt = reader.GetAttribute("skytext");
                            _type = ConvertType(skyTxt);
                            forcast.Append("Now ").Append(_Temp).Append(" ").Append(skyTxt).Append(Environment.NewLine);
                            break;
                        case "forecast":
                            string low = reader.GetAttribute("low");
                            string high = reader.GetAttribute("high");
                            string skyText = reader.GetAttribute("skytextday");
                            DateTime forcastDay = DateTime.Parse(reader.GetAttribute("date"));
                            string day = reader.GetAttribute("day");
                            string percept = reader.GetAttribute("precip");
                            if (string.IsNullOrWhiteSpace(percept)) { percept = "0"; }
                            if (forcastDay >= DateTime.Today) { forcast.Append(string.Format(forcastFormat, low, high, skyText, day, percept)).Append(Environment.NewLine); }
                            break;
                        case "toolbar":
                            int Timeout = int.Parse(reader.GetAttribute("timewindow"));
                            //if (Timeout > _cacheTimeout) _cacheTimeout = Timeout;
                            break;
                    }
            }
            _forcast = forcast.ToString();
        }
        private static WeatherTypes ConvertType(string SkyText)
        {
            if (SkyText.IndexOf("thunderstorm", StringComparison.OrdinalIgnoreCase) != -1) { return WeatherTypes.ThunderStorm; }
            else if (SkyText.IndexOf("rain", StringComparison.OrdinalIgnoreCase) != -1) { return WeatherTypes.Rain; }
            else if (SkyText.IndexOf("snow", StringComparison.OrdinalIgnoreCase) != -1) { return WeatherTypes.Snow; }
            else if (SkyText.IndexOf("partly", StringComparison.OrdinalIgnoreCase) != -1) { return WeatherTypes.PartlyCloudy; }
            else
            {
                switch (SkyText.ToLower())
                {
                    case "cloudy": return WeatherTypes.Cloudy;
                    case "dust": return WeatherTypes.Dust;
                    case "fog": return WeatherTypes.Fog;
                    case "showers": return WeatherTypes.Rain;
                    case "haze": return WeatherTypes.Haze;
                    case "smoke": return WeatherTypes.Smoke;
                    case "windy": return WeatherTypes.Windy;
                    case "Frigid": return WeatherTypes.Frigid;
                    case "Hot": return WeatherTypes.Hot;
                    default: return WeatherTypes.Clear;
                }
            }

        }
    }
}
