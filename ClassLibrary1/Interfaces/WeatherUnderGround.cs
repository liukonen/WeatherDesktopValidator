using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WeatherDesktopSharedCore.Interfaces
{
    public class WeatherUnderGround :BaseBoth
    {
        const string wunderground_Url = "http://api.wunderground.com/api/{1}/conditions/astronomy/q/{0}.xml";
        const string APIKey = "";

        private Boolean _Success;
        private int _Temp;
        private string _err = string.Empty;
        private string _forcast;
        private WeatherTypes _type;
        public override string ToString()
        {
            return "Weather Underground";
        }
        private DateTime _solarNoon;
        private DateTime _sunRise;
        private DateTime _sunSet;
        private string _status;

        public override string ErrorMessages() { return _err; }
        public override string ForcastDescription() { return _forcast; }
        public override bool Success() { return _Success; }
        public override int Temp() { return _Temp; }
        public override WeatherTypes WType() { return _type; }

        public override DateTime SolarNoon() { return _solarNoon; }
        public override string Status() { return _status; }
        public override DateTime SunRise() { return _sunRise; }
        public override DateTime SunSet() { return _sunSet; }

        public override void Invoke()
        {
            try {
                string item = CompressedCallSite(string.Format(wunderground_Url, ZipCode, APIKey));
                _Success = true;
            }
            catch (Exception x) { _Success = false; _err = x.ToString(); }

        }



        private void transformXML(string xml)
        {
            int SunsetHour = 0, SunriseHour = 0, SunsetMinute = 0, SunRiseMinute = 0;
            int activeType = 0;

            string forcast = string.Empty;
            string currentTemp = string.Empty;
            //WeatherResponse value = new WeatherResponse();
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(xml));
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element))
                    switch (reader.Name)
                    {
                        case "latitude":
                            //LatLong.Latitude = double.Parse(reader.ReadInnerXml());
                            break;
                        case "longitude":
                            //LatLong.Longitude = double.Parse(reader.ReadInnerXml());
                            break;
                        case "weather":
                            forcast = reader.ReadInnerXml();
                            break;
                        case "temperature_string":
                            currentTemp = reader.ReadInnerXml();
                            break;
                        case "temp_f":
                            _Temp = Convert.ToInt32(double.Parse(reader.ReadInnerXml()));
                            break;
                        case "icon_url":
                            string _url = reader.ReadInnerXml();
                            _type = ConvertImageToType(_url);
                            break;
                        case "sunset":
                            activeType = 1;
                            break;
                        case "sunrise":
                            activeType = 2;
                            break;
                        case "hour":
                            int TestHour = 0;
                            if (!int.TryParse(reader.ReadInnerXml(), out TestHour)) { TestHour = 0; };
                            if (activeType > 0)
                            {
                                if (activeType == 1 && SunsetHour == 0) { SunsetHour = (TestHour > 0) ? TestHour : 1; }
                                else if (SunriseHour == 0) { SunriseHour = (TestHour > 0) ? TestHour : 1; }
                            }
                            break;
                        case "minute":
                            int TestMin = 0; int.TryParse(reader.ReadInnerXml(), out TestMin);
                            if (activeType > 0)
                            {
                                if (activeType == 1 && SunsetMinute == 0) { SunsetMinute = (TestMin > 0) ? TestMin : 1; }
                                else if (SunRiseMinute == 0) { SunRiseMinute = (TestMin > 0) ? TestMin : 1; }

                            }
                            break;
                    }
            }
            _forcast = forcast + " " + currentTemp;
           _status = "ok";
            _sunRise = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, SunriseHour, SunRiseMinute, 0);
            _sunSet = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, SunsetHour, SunsetMinute, 0);
        }

        private static WeatherTypes ConvertImageToType(string url)
        {
            string parsed = url.Substring(url.LastIndexOf("/") + 1).Replace(".gif", string.Empty).Replace("nt_", string.Empty);
            switch (parsed)
            {
                case "chanceflurries":
                case "chancesleet":
                case "chancesnow":
                case "sleet":
                case "snow":
                case "flurries":
                    return WeatherTypes.Snow;
                case "chancerain":
                case "rain":
                    return WeatherTypes.Rain;
                case "chancetstorms":
                case "tstorm":
                    return WeatherTypes.ThunderStorm;
                case "cloudy":
                case "mostlycloudy":
                    return WeatherTypes.Cloudy;
                case "partlycloudy":
                case "partlysunny":
                    return WeatherTypes.PartlyCloudy;
                case "fog":
                    return WeatherTypes.Fog;
                case "hazy":
                    return WeatherTypes.Haze;
                case "clear":
                case "mostlysunny":
                    return WeatherTypes.Clear;
                default:
                    return WeatherTypes.Windy;
            }

        }
    }
}

