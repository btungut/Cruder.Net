using System;

namespace Cruder.Resource
{
    public static class ResourceManager
    {
        //Namespace.FileName
        private static readonly System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Cruder.Resource.Resources", typeof(ResourceManager).Assembly);

        public static string GetString(string key)
        {
            string retVal = null;

            try
            {
                retVal = manager.GetString(key);
            }
            catch (Exception e)
            {
                var exception = new Exception("Resource not found for '" + key + "' key.", e);
            }

            return retVal;
        }
    }
}
