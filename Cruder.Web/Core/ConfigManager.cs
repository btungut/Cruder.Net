using Cruder.Core.Contract;
using Cruder.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cruder.Web
{
    public static class ConfigManager
    {
        public static string AppName
        {
            get
            {
                return GetValue("AppName");
            }
        }

        private static List<ConfigEntity> values = null;
        public static List<ConfigEntity> Values
        {
            get
            {
                if (values == null)
                {
                    Initialize();
                }

                return values;
            }
        }

        private static ICruderRepository<ConfigEntity> GetRepository()
        {
            return Cruder.Core.Module.IoC.Resolve<ICruderRepository<ConfigEntity>>();
        }

        public static void Initialize()
        {
            values = GetRepository().Query().ToList();
        }

        public static string GetValue(string key, bool firstlyLookupInternal = true)
        {
            string retVal = null;

            if (firstlyLookupInternal)
            {
                if (Values.Any(q => q.Key == key))
                {
                    retVal = GetValueByEnvironment(Values.Single(q => q.Key == key));
                }
                else
                {
                    retVal = GetValueByEnvironment(GetEntityFromDatabase(key));
                }
            }
            else
            {
                retVal = GetValueByEnvironment(GetEntityFromDatabase(key));
            }

            return retVal;
        }

        private static ConfigEntity GetEntityFromDatabase(string key)
        {
            var entity = GetRepository().Query(q => q.Key == key).SingleOrDefault();

            UpdateInternalValue(entity);

            return entity;
        }

        private static void UpdateInternalValue(ConfigEntity entity)
        {
            if (entity != null)
            {
                if (Values.Any(q => q.Key == entity.Key))
                {
                    var get = Values.Single(q => q.Key == entity.Key);
                    get.ProductionValue = entity.ProductionValue;
                    get.DevelopmentValue = entity.DevelopmentValue;
                }
                else
                {
                    Values.Add(entity);
                }
            }
        }

        private static string GetValueByEnvironment(ConfigEntity entity)
        {
            if (entity == null) return null;

            if (Definition.IsDevelopmentEnvironment)
            {
                return entity.DevelopmentValue;
            }
            else
            {
                return entity.ProductionValue;
            }
        }

    }
}
