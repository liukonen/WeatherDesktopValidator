﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeatherDesktopSharedCore.Interfaces
{
    public class YahooSun :BaseSun
    {
        private const string Yahoo_SRS_Url = "https://query.yahooapis.com/v1/public/yql?q=select%20astronomy%20from%20weather.forecast%20where%20woeid%20in%20(select%20content%20from%20pm.location.zip.region%20where%20zip%3D%22{0}%22%20and%20region%3D%22us%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
        private Boolean _Success;
        private string _err;

        private DateTime _solarNoon;
        private DateTime _sunRise;
        private DateTime _sunSet;
        private string _status;
        public override string ToString()
        {
            return "Yahoo Sun Rise Set";
        }
        public override string ErrorMessages() { return _err; }
        public override bool Success() { return _Success; }

        public override DateTime SolarNoon() { return _solarNoon; }
        public override string Status() { return _status; }
        public override DateTime SunRise() { return _sunRise; }
        public override DateTime SunSet() { return _sunSet; }

        public override void Invoke()
        {
            try {
                LiveCall();
                _Success = true;
            }

            catch (Exception x) { _err = x.ToString(); _Success = false; }
        }

        private void LiveCall()
        {
                string URL = string.Format(Yahoo_SRS_Url, ZipCode);
                string results = CompressedCallSite(URL);
                
                YahooSRSObject Response = JsonConvert.DeserializeObject<YahooSRSObject>(results); // jsSerialization.Deserialize<YahooSRSObject>(results);
                _sunRise = DateTime.ParseExact(Response.query.results.channel.astronomy.sunrise, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                _sunSet = DateTime.ParseExact(Response.query.results.channel.astronomy.sunset, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                _status = "ok";
            

        }

        #region AutogeneratedCode
        [DataContract]
        public class Astronomy
        {

            [DataMember(Name = "sunrise")]
            public string sunrise { get; set; }

            [DataMember(Name = "sunset")]
            public string sunset { get; set; }
        }

        [DataContract]
        public class Channel
        {

            [DataMember(Name = "astronomy")]
            public Astronomy astronomy { get; set; }
        }

        [DataContract]
        public class Results
        {

            [DataMember(Name = "channel")]
            public Channel channel { get; set; }
        }

        [DataContract]
        public class Query
        {

            [DataMember(Name = "count")]
            public int count { get; set; }

            [DataMember(Name = "created")]
            public DateTime created { get; set; }

            [DataMember(Name = "lang")]
            public string lang { get; set; }

            [DataMember(Name = "results")]
            public Results results { get; set; }
        }

        [DataContract]
        public class YahooSRSObject
        {

            [DataMember(Name = "query")]
            public Query query { get; set; }
        }

#endregion
    }
}