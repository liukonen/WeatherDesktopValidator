using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WeatherDesktopSharedCore.Interfaces
{
    public class NOAAV2 :BaseWeather
    {
        const string Gov_Weather_Url = "https://graphical.weather.gov/xml/sample_products/browser_interface/ndfdXMLclient.php?zipCodeList={0}&maxt=maxt&mint=mint&wx=wx&icons=icons";
        const string Gov_User = "WeatherWallpaper/v1.0 (https://github.com/liukonen/WeatherWallpaper/; liukonen@gmail.com)";
        private Boolean _Success;
        private int _Temp;
        private string _err;
        private string _forcast;
        private WeatherTypes _type;

        public override string ErrorMessages() { return _err; }
        public override string ForcastDescription() { return _forcast; }
        public override bool Success() { return _Success; }
        public override int Temp() { return _Temp; }
        public override WeatherTypes WType() { return _type; }
        public override string ToString()
        {
            return "Weather.Gov";
        }
        public override void Invoke()
        {
            try
            {
                string httpResponse = CompressedCallSite(string.Format(Gov_Weather_Url, ZipCode), Gov_User);
                Transform(httpResponse);
                _Success = true;
            }
            catch (Exception x) { _Success = false; _err = x.ToString(); }
        }

        void Transform(string Response)
        {
            int Max = -180;
            int Min = 180;

           // WeatherResponse value = new WeatherResponse();
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(Response));
            string ForcastType = string.Empty;
            string type = string.Empty;
            string iconUrl = string.Empty;
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element))
                    switch (reader.Name)
                    {
                        case "point":
                            //if (!SharedObjects.LatLong.HasRecord())
                            //{
                            //    double lat, lng;
                            //    if (double.TryParse(reader.GetAttribute("latitude"), out lat) && double.TryParse(reader.GetAttribute("longitude"), out lng))
                            //    { SharedObjects.LatLong.Set(lat, lng); }
                            //}
                            break;
                        case "temperature":
                            type = reader.GetAttribute("type");
                            break;
                        case "value":
                            if (type == "maximum")
                            {
                                int test;
                                test = int.Parse(reader.ReadInnerXml());
                                if (Max == -180) { _Temp = test; }
                                Max = (test > Max) ? test : Max;
                            }
                            else if (type == "minimum")
                            {
                                int test;
                                test = int.Parse(reader.ReadInnerXml());

                                Min = (test > Min) ? Min : test;
                            }
                            else if (type == "Weather")
                            {
                                if (string.IsNullOrEmpty(ForcastType))
                                {
                                    string coverage = reader.GetAttribute("coverage");
                                    string intensity = reader.GetAttribute("intensity");
                                    string additive = reader.GetAttribute("additive");
                                    ForcastType = reader.GetAttribute("weather-type");
                                    _forcast = string.Concat(coverage, " ", additive, (string.IsNullOrWhiteSpace(additive) ? " " : ""), ForcastType, ((intensity == "none") ? string.Empty : " (" + intensity + ")"));
                                }
                            }

                            //type = string.Empty;
                            break;
                        case "weather-conditions":
                            type = "Weather";
                            break;
                        case "icon-link":
                            if (string.IsNullOrWhiteSpace(iconUrl)) { iconUrl = reader.ReadInnerXml(); }
                            break;

                    }
            }
            _forcast += string.Concat("(", Max.ToString(), "-", Min.ToString(), ")");
            _type = extractWeatherType(ForcastType, iconUrl);
        }

        /// <summary>
        /// Extracts the weather type from the current type, or if it can't find it, from the weather icon url.
        /// </summary>
        /// <param name="currentType"></param>
        /// <param name="Urlbackup"></param>
        /// <returns></returns>
        static WeatherTypes extractWeatherType(string currentType, string Urlbackup)
        {
            switch (currentType)
            {
                case "thunderstorms":
                case "water spouts":
                    return WeatherTypes.ThunderStorm;
                case "snow shower":
                case "blowing snow":
                case "frost":
                case "snow":
                    return WeatherTypes.Snow;
                case "freezing spray":
                case "ice crystals":
                case "ice pellets":
                case "freezing fog":
                case "ice fog":
                    return WeatherTypes.Frigid;
                case "freezing drizzle":
                case "freezing rain":
                case "drizzle":
                case "rain":
                case "rain shower":
                case "hail":
                    return WeatherTypes.Rain;
                case "fog":
                    return WeatherTypes.Fog;
                case "haze":
                    return WeatherTypes.Haze;
                case "smoke":
                case "volcanic ash":
                    return WeatherTypes.Smoke;
                case "blowing dust":
                case "blowing sand":
                    return WeatherTypes.Dust;
            }
            return ExtractTypeFromIcon(Urlbackup);
        }

        /// <summary>
        /// takes the image that would normally be used for an app, and extracts its "weather type"
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static WeatherTypes ExtractTypeFromIcon(string url)
        {
            string core = url.Substring(url.LastIndexOf("/")).Replace(".jpg", string.Empty);
            if (core.StartsWith("ntsra") || core.StartsWith("tsra")) return WeatherTypes.ThunderStorm;  //night Thunderstorm
            if (core.StartsWith("nscttsra") || core.StartsWith("scttsra")) return WeatherTypes.ThunderStorm; //night sky thunderstorm
            if (core.StartsWith("ip")) return WeatherTypes.Snow;  //Ice Particals
            if (core.StartsWith("nraip") || core.StartsWith("raip")) return WeatherTypes.Rain; // night rain  / Ice particals
            if (core.StartsWith("mix")) return WeatherTypes.Snow;
            if (core.StartsWith("nrasn")) return WeatherTypes.Rain;//snow rain
            if (core.StartsWith("nsn") || core.StartsWith("sn")) return WeatherTypes.Snow;
            if (core.StartsWith("fzra")) return WeatherTypes.Rain; //freezing rain
            if (core.StartsWith("nra") || core.StartsWith("ra")) return WeatherTypes.Rain; // night rain.
            if (core.StartsWith("hi_nshwrs") || core.StartsWith("shra") || core.StartsWith("hi_shwrs")) return WeatherTypes.Rain; //showers
            if (core == "blizzard") return WeatherTypes.Snow;
            if (core == "du") return WeatherTypes.Dust;
            if (core == "fu") return WeatherTypes.Smoke; //patchy or smoke
            switch (core)
            {
                case "nfg":
                case "fg":
                    return WeatherTypes.Fog;
                case "nwind":
                case "wind":
                    return WeatherTypes.Windy;
                case "novc":
                case "ovc":
                case "nbkn":
                case "bkn":
                    return WeatherTypes.Cloudy;
                case "nsct":
                case "sct":
                case "nfew":
                case "few":
                    return WeatherTypes.PartlyCloudy;
                case "nskc":
                case "skc":
                    return WeatherTypes.Clear;
                case "cold":
                    return WeatherTypes.Frigid;
                case "hot":
                    return WeatherTypes.Hot;

            }
            return WeatherTypes.Clear;
        }

    }
}
