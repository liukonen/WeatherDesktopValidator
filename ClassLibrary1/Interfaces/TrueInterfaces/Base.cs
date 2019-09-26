using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Runtime.Caching;

namespace WeatherDesktopSharedCore.Interfaces
{
    public abstract class Base
    {
        
     

        public string ZipCode = "53217";
        public enum WeatherTypes { ThunderStorm, Rain, Snow, Dust, Fog, Haze, Smoke, Windy, Frigid, Cloudy, PartlyCloudy, Clear, Hot };

        public static string CompressedCallSite(string Url, string UserAgent)
        {
            if (Cache.Exists(string.Concat(UserAgent, Url))) { return Cache.StringValue(string.Concat(UserAgent, Url)); }

            HttpWebRequest request = (System.Net.HttpWebRequest)HttpWebRequest.Create(Url);
            if (!string.IsNullOrWhiteSpace(UserAgent)) { request.UserAgent = UserAgent; }
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.AllowAutoRedirect = true;
            string strresponse;
            using (System.Net.WebResponse response = request.GetResponse())
            {
                StreamReader Reader = new System.IO.StreamReader(response.GetResponseStream());
                strresponse =  Reader.ReadToEnd();
            }
            Cache.Set(string.Concat(UserAgent, Url), strresponse);
            return strresponse;
        }

        public static string CompressedCallSite(string Url)
        {
            return CompressedCallSite(Url, string.Empty);
        }
    }

    public abstract class BaseWeather : Base, Iweather
    {
        public abstract string ErrorMessages();
        public abstract string ForcastDescription();
        public abstract void Invoke();
        public abstract bool Success();
        public abstract int Temp();
        public abstract WeatherTypes WType();
    }

    public abstract class BaseSun : Base, Isrs
    {
        public abstract string ErrorMessages();
        public abstract void Invoke();
        public abstract DateTime SolarNoon();
        public abstract string Status();
        public abstract bool Success();
        public abstract DateTime SunRise();
        public abstract DateTime SunSet();
    }

    public abstract class BaseBoth : Base, IBoth
    {
        public abstract string ErrorMessages();
        public abstract string ForcastDescription();
        public abstract void Invoke();
        public abstract DateTime SolarNoon();
        public abstract string Status();
        public abstract bool Success();
        public abstract DateTime SunRise();
        public abstract DateTime SunSet();
        public abstract int Temp();
        public abstract WeatherTypes WType();
    }


    public static class Cache
    {

        private static string TransformKey(string key)
        { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" + key; }

        public static object Value(string key)
        {
            string NewKey = TransformKey(key);
            System.Runtime.Caching.MemoryCache cache = System.Runtime.Caching.MemoryCache.Default;
            if (cache.Contains(NewKey)) return cache[NewKey];
            return string.Empty;
        }

        public static string StringValue(string key)
        {
            return (string)Value(key);
        }

        public static bool Exists(string key)
        {
            System.Runtime.Caching.MemoryCache cache = System.Runtime.Caching.MemoryCache.Default;
            return cache.Contains(TransformKey(key));
        }

        public static void Set(string key, object o, int timeout)
        {
            System.Runtime.Caching.MemoryCache cache = System.Runtime.Caching.MemoryCache.Default;
            cache.Add(TransformKey(key), o, DateTime.Now.AddMinutes(timeout));
        }
        public static void Set(string key, object o)
        {
            Set(key, o, 15);
        }

    }
}
