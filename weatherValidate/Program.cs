using System;
using System.Collections;
using System.Collections.Generic;
using WeatherDesktopSharedCore.Interfaces;
using WeatherDesktopSharedCore;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;

namespace weatherValidate
{
    class Program
    {
        static void Main(string[] args)
        {

            var SunObjects = new List<Isrs>();
            var WeatherObjects = new List<Iweather>();
            List<ISharedResponse> items = new List<WeatherDesktopSharedCore.Interfaces.ISharedResponse>();
            items.Add(new MSWeather());
            items.Add(new NOAAV2());
            items.Add(new OpenWeather() { APIKey = Properties.Resources.OpenWeatherAPIKey });
            items.Add(new SunRiseSet());
            items.Add(new WeatherAPI3());
            //items.Add(new WeatherUnderGround());

            System.Threading.Tasks.ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 4;
            Parallel.ForEach(items, o, (currentFile) => currentFile.Invoke());
            foreach (var item in items)
            {

                if (!item.Success()) 
                {
                    Console.WriteLine(item.ErrorMessages());
                   // PostToIfTTT(item.GetType().Name, item.ErrorMessages(), item.PostUrl());
                }
                else {

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(item.GetType().Name + ": OK - ");
                    if (item is Iweather) {
                        var XX = (Iweather)item;
                        sb.Append("temp:").Append(XX.Temp().ToString()).Append(", ");
                        //sb.Append("forcast:").Append(XX.ForcastDescription()).Append(", ");
                        sb.Append("Type:").Append(Enum.GetName(XX.WType().GetType(), XX.WType())).Append(", ");
                    }
                    if (item is Isrs) 
                    {
                        var XX = (Isrs)item;
                        sb.Append("").Append(XX.SunRise().ToShortTimeString()).Append(", ");
                        sb.Append("").Append(XX.SunSet().ToShortTimeString()).Append(", ");
                    }



                    Console.WriteLine(sb.ToString());
                }
                //if (item is Isrs)
                //{ SunObjects.Add((Isrs)item); }
                //if (item is Iweather)
                //{ WeatherObjects.Add((Iweather)item); }
            }







        }

        private static void PostToIfTTT(string className, string Error, string posturl)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Properties.Resources.IFtttUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                dynamic item = new JObject();
                item.value1 = className;
                item.value2 = Error;
                item.value3 = posturl;
                streamWriter.Write(item.ToString());
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}
