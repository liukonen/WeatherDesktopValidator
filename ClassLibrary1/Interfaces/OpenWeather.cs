using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace WeatherDesktopSharedCore.Interfaces
{
    public class OpenWeather : BaseBoth
    {

        private Boolean _Success;
        private int _Temp;
        private string _err = string.Empty;
        private string _forcast;
        private WeatherTypes _type;
        public override string ToString()
        {
            return "Open Weather";
        }
        private string _url;
        private DateTime _solarNoon;
        private DateTime _sunRise;
        private DateTime _sunSet;
        private string _status;
 

        public override string ErrorMessages() { return _err; }
        public override string ForcastDescription() { return _forcast; }
        public override bool Success() { return _Success; }
        public override int Temp() { return _Temp; }
        public override WeatherTypes WType() { return _type; }
        public override string PostUrl()
        {
            return _url;
        }
        public override DateTime SolarNoon() { return _solarNoon; }
        public override string Status() { return _status; }
        public override DateTime SunRise() { return _sunRise; }
        public override DateTime SunSet() { return _sunSet; }


        public override void Invoke()
        {
            try
            {
                string value = GetValue;
                //JavaScriptSerializer jsSerialization = new JavaScriptSerializer();
                OpenWeatherMapObject weatherObject = JsonConvert.DeserializeObject<OpenWeatherMapObject>(value); //jsSerialization.Deserialize<OpenWeatherMapObject>(value);
                _Temp = (int)weatherObject.main.temp;
                _forcast = GenerateForcast(weatherObject.main, weatherObject.weather[0]);
                _type = GetWeatherType(weatherObject.weather[0].id);

                _sunRise = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(weatherObject.sys.sunrise).ToLocalTime();
                _sunSet = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(weatherObject.sys.sunset).ToLocalTime();
                _solarNoon = DateTime.Now;
                _status = string.Empty;
                _Success = true;
            }
            catch (Exception x) { _err = x.ToString(); _Success = false; }

        }

        const string OpenWeather_Url = "http://api.openweathermap.org/data/2.5/weather?zip={0}&appid={1}&units=imperial";
        public string APIKey;
        private string GetValue
        {
            get
            {
                
                string url = string.Format(OpenWeather_Url, ZipCode, APIKey);
                _url = url;
                string value = CompressedCallSite(url);
                return value;
            }
        }


        private string GenerateForcast(Main Mainweather, Weather WeatherObject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Mainweather.temp).Append(",  ").Append(WeatherObject.description).Append(Environment.NewLine);
            sb.Append("Humidity: ").Append(Mainweather.humidity).Append(" Range: ").Append(Mainweather.temp_min).Append("-").Append(Mainweather.temp_max);
            return sb.ToString();
        }


        private WeatherTypes GetWeatherType(int ParseItem)
        {
            double value = ParseItem / 100;

            //first the groups
            switch ((int)Math.Floor(value))
            {
                case 2:
                case 960:
                case 961:
                    return WeatherTypes.ThunderStorm;
                case 3:
                case 5:
                    return WeatherTypes.Rain;
                case 6:
                    return WeatherTypes.Snow;
            }


            switch (ParseItem)
            {
                case 701:
                    return WeatherTypes.Rain;
                case 711:
                    return WeatherTypes.Smoke;
                case 721:
                    return WeatherTypes.Haze;
                case 741:
                    return WeatherTypes.Fog;
                case 731:
                case 751:
                case 761:
                case 762:
                    return WeatherTypes.Dust;
                case 800:
                case 951:
                case 952:
                case 953:
                case 955:
                    return WeatherTypes.Clear;
                case 801:
                case 802:
                    return WeatherTypes.PartlyCloudy;
                case 803:
                case 804:
                    return WeatherTypes.Cloudy;
                case 903:
                    return WeatherTypes.Frigid;
                case 904:
                    return WeatherTypes.Hot;
                case 905:
                case 954:
                case 956:
                case 957:
                case 958:
                    return WeatherTypes.Windy;

            }
            return WeatherTypes.ThunderStorm;// In the act of Some of the Extremes I did not cover... Thumderstorm it is
            //list of items directly not covered: 771 squalls, 781 tornado, 900 tornado, 901 tropical storm, 902 hurricane, 906 hail, 959 severe gale, 962 hurrican
        }


#pragma warning disable 0649
        #region Auto Generated Code
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a tool.
        //     Runtime Version:4.0.30319.42000
        //
        //     Changes to this file may cause incorrect behavior and will be lost if
        //     the code is regenerated.
        // </auto-generated>
        //------------------------------------------------------------------------------



        // Type created for JSON at <<root>>
        [System.Runtime.Serialization.DataContractAttribute()]
        public partial class OpenWeatherMapObject
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Coord coord;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Sys sys;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Weather[] weather;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string @base;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Main main;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Wind wind;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public Clouds clouds;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int dt;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int id;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string name;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int cod;
        }

        // Type created for JSON at <<root>> --> coord
        [System.Runtime.Serialization.DataContractAttribute(Name = "coord")]
        public partial class Coord
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double lon;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double lat;
        }

        // Type created for JSON at <<root>> --> sys
        [System.Runtime.Serialization.DataContractAttribute(Name = "sys")]
        public partial class Sys
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int type;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int id;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double message;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string country;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int sunrise;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int sunset;
        }

        // Type created for JSON at <<root>> --> weather
        [System.Runtime.Serialization.DataContractAttribute(Name = "weather")]
        public partial class Weather
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int id;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string main;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string description;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string icon;
        }

        // Type created for JSON at <<root>> --> main
        [System.Runtime.Serialization.DataContractAttribute(Name = "main")]
        public partial class Main
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double temp;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int humidity;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double pressure;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double temp_min;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double temp_max;
        }

        // Type created for JSON at <<root>> --> wind
        [System.Runtime.Serialization.DataContractAttribute(Name = "wind")]
        public partial class Wind
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double speed;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double deg;
        }

        // Type created for JSON at <<root>> --> clouds
        [System.Runtime.Serialization.DataContractAttribute(Name = "clouds")]
        public partial class Clouds
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int all;
        }
        #endregion
#pragma warning restore 0649
    }
}
