using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherDesktopSharedCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WeatherDesktopValidator.Pages
{
    public class IndexModel : PageModel
    {
        public string WeatherUndergroundAPIKey { get; }

        public void OnGet()
        {
            SunObjects = new List<Isrs>();
            WeatherObjects = new List<Iweather>();
            List<ISharedResponse> items = new List<ISharedResponse>();
            items.Add(new NOAAV2());
            items.Add(new OpenWeather() { APIKey = WeatherUndergroundAPIKey });
            items.Add(new SunRiseSet());

            System.Threading.Tasks.ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 4;
            Parallel.ForEach(items, o, (currentFile) => currentFile.Invoke());
            foreach (var item in items)
            {
                if (item is Isrs)
                { SunObjects.Add((Isrs)item); }
                if (item is Iweather)
                { WeatherObjects.Add((Iweather)item); }
            }

        }
        public List<Iweather> WeatherObjects { get; set; }
        public List<Isrs> SunObjects { get; set; }

        //public IndexModel(IConfiguration configuration)
        //{
        //    WeatherUndergroundAPIKey = configuration["MySettings:OpenWeather"];
        //}
    }
}
