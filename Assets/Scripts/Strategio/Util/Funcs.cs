using System;
using UnityEngine;

namespace Strategio.Util
{
    public static class Funcs
    {
        public static T Log<T>(this T t,Func<T,string> format)
        {
            Debug.Log(format(t));
            return t;
        }
    }
}