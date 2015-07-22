using System;

namespace Cruder.Core.Module
{
    public static class JsonHelper
    {
        public static string Serialize(object obj)
        {
            string retVal = "";

            try
            {
                retVal = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, Priority.Normal, "JsonHelper.ToSerialize()", e, LogModule.Framework);
            }

            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T retVal = default(T);

            try
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, Priority.Normal, "JsonHelper.ToDeserialize<>()", e, LogModule.Framework);
            }

            return retVal;
        }
    }
}
