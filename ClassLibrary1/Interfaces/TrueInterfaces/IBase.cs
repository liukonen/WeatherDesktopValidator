using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherDesktopSharedCore.Interfaces
{
    public interface ISharedResponse { void Invoke();  string ErrorMessages(); Boolean Success(); string PostUrl(); }

    public interface Isrs : ISharedResponse
    {
        DateTime SunRise();
        DateTime SunSet();
        DateTime SolarNoon();
        string Status();
     
    }

    public interface Iweather : ISharedResponse
    {
        Base.WeatherTypes WType();
        int Temp();
        string ForcastDescription();
    }

    interface IBoth : Iweather, Isrs
    { }
}
