using System;

namespace Cruder.Helper
{
    public static class ConvertionHelper
    {
        public static object GetConvertedObject(object value, Type type, System.Globalization.CultureInfo culture)
        {
            return Convert.ChangeType(value, type, culture);
        }
    }
}
