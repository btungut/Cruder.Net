using Cruder.Core.ExceptionHandling;
using System;
using System.Text.RegularExpressions;

namespace Cruder.Helper
{
    public static class ObjectHelper
    {
        public static string ToJson(this object obj)
        {
            return Cruder.Core.Module.JsonHelper.Serialize(obj);
        }

        public static T ToConvert<T>(this object value)
        {
            return ToConvert<T>(value, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public static T ToConvert<T>(this object value, string culture)
        {
            return ToConvert<T>(value, new System.Globalization.CultureInfo(culture));
        }

        public static T ToConvert<T>(this object value, System.Globalization.CultureInfo culture)
        {
            return (T)ConvertionHelper.GetConvertedObject(value, typeof(T), culture);
        }

        public static string ToStringWithFormat(this object obj, string format)
        {
            try
            {
                Regex regex = new Regex(@"#{[a-zA-Z0-9-.]+}");
                var matches = regex.Matches(format);

                foreach (Match match in matches)
                {
                    string key = match.Value.Substring(2, match.Value.Length - 3); //Remove first and last char

                    object memberObj = obj;
                    Type memberType = obj.GetType();

                    foreach (var item in key.Split('.'))
                    {
                        var property = memberType.GetProperty(item);
                        var field = memberType.GetField(item);

                        if (property != null)
                        {
                            memberType = property.PropertyType;
                            memberObj = property.GetValue(memberObj, null);
                        }
                        else if (field != null)
                        {
                            memberType = field.FieldType;
                            memberObj = field.GetValue(memberObj);
                        }
                        else
                        {
                            throw new Exception(string.Format("Unable to find any member with name of '{0}'.", item));
                        }
                    }

                    format = format.Replace(match.Value, memberObj == null ? string.Empty : memberObj.ToString());
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("ObjectHelper.ToStringWithFormat()", "An error occured while formatting object with format string.", e);
                exception.Data.Add("format", format);
                throw exception;
            }

            return format;
        }
    }
}
