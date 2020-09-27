using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WeatherDesktopSharedCore.Interfaces;
using weatherValidate.Properties;

namespace weatherValidate
{
    class Program
    {
 
        private static void Main(string[] args)
        {
            new List<Isrs>();
            new List<Iweather>();
            List<ISharedResponse> obj = new List<ISharedResponse>
            {
                new MSWeather(),
                new NOAAV2(),
                new OpenWeather
                {
                    APIKey = Resources.OpenWeatherAPIKey
                },
                new SunRiseSet()
            };
            Parallel.ForEach((IEnumerable<ISharedResponse>)obj, new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            }, (Action<ISharedResponse>)delegate (ISharedResponse currentFile)
            {
                currentFile.Invoke();
            });
            foreach (ISharedResponse item in obj)
            {
                if (!item.Success())
                {
                    Console.WriteLine(item.ErrorMessages());
                    PostToIfTTT(item.GetType().Name, item.ErrorMessages(), item.PostUrl());
                }
                else
                {
                    Console.WriteLine(item.GetType().Name + ": OK");
                }
            }
        }

        private static void PostToIfTTT(string className, string Error, string posturl)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Resources.IFtttUrl);
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
