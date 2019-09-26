using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherDesktopSharedCore.Interfaces;

namespace WeatherDesktopValidator.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            WeatherObjects = new List<Iweather>();
            SunObjects = new List<Isrs>();
            List<ISharedResponse> items = new List<WeatherDesktopSharedCore.Interfaces.ISharedResponse>();
            items.Add(new YahooSun());
            items.Add(new YahooWeather());
            items.Add(new MSWeather());
            items.Add(new NOAAV2());
            items.Add(new OpenWeather());
            items.Add(new WeatherUnderGround());
            items.Add(new SunRiseSet());

            System.Threading.Tasks.ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 4;
            Parallel.ForEach(items, o, (currentFile) => currentFile.Invoke());
            foreach (var item in items)
            {
            //    item.Invoke();
                if (item is Isrs)
                { SunObjects.Add((Isrs)item); }
                if (item is Iweather)
                { WeatherObjects.Add((Iweather)item); }
            }


            Responses = BuildCard();
        }

        public string Responses { get; set; }



        public List<WeatherDesktopSharedCore.Interfaces.Iweather> WeatherObjects { get; set; }
        public List<WeatherDesktopSharedCore.Interfaces.Isrs> SunObjects { get; set; }

        public static string BuildCard()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<div class=""col-3"">");
            sb.Append("Hello Luke");

            sb.Append("</div>");

            return sb.ToString();

        }
    }
}
