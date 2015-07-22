using Cruder.Core;
using Cruder.Core.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cruder.Helper
{
    public static class EnumHelper
    {
        public static string GetName<T>(object value)
        {
            string retVal;

            try
            {
                retVal = Enum.GetName(typeof(T), value);
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("EnumHelper.GetName<>()", "An error occured while getting enum name.", e);
                exception.Data.Add("Type", typeof(T).FullName);
                exception.Data.Add("Value", value);
                throw exception;
            }

            return retVal;
        }

        public static T GetEnum<T>(object value)
        {
            T retVal;
            try
            {
                retVal = (T)Enum.Parse(typeof(T), value.ToString());
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("EnumHelper.GetEnum<>()", "An error occured while getting enum.", e);
                exception.Data.Add("T", typeof(T).FullName);
                exception.Data.Add("Value", value);
                throw exception;
            }
            return retVal;
        }

        public static T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            T retVal = null;

            try
            {
                System.Reflection.MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).SingleOrDefault();

                if (memberInfo != null)
                {
                    retVal = (T)memberInfo.GetCustomAttributes(typeof(T), false).SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("EnumHelper.GetAttribute<>()", "An error occured while getting enum attribute.", e);
                exception.Data.Add("T", typeof(T).FullName);
                exception.Data.Add("EnumValue", enumValue);
                throw exception;
            }

            return retVal;
        }

        public static string GetDetail(Enum enumValue)
        {
            var detail = GetAttribute<DetailAttribute>(enumValue);

            if (detail == null)
            {
                throw new ArgumentNullException("That enum member has not detail attribute!");
            }

            return detail.Value;
        }

        public static string GetDetail<T>(int value)
        {
            return GetDetail((Enum)Enum.Parse(typeof(T), value.ToString()));
        }

        public static Dictionary<int, string> ConvertToDictionary<T>()
        {
            Dictionary<int, string> retVal;

            try
            {
                var type = typeof(T);
                retVal = Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => EnumHelper.GetName<T>(e));
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("EnumHelper.ConvertToDictionary<>()", "An error occured while enum converting to dictionary.", e);
                exception.Data.Add("T", typeof(T).FullName);
                throw exception;
            }

            return retVal;
        }

        public static List<Cruder.Core.Model.EnumerationModel> ConvertToList<T>()
        {
            List<Cruder.Core.Model.EnumerationModel> retVal = new List<Cruder.Core.Model.EnumerationModel>();
            var enumType = typeof(T);

            try
            {
                foreach (var name in Enum.GetValues(typeof(T)))
                {
                    var item = new Cruder.Core.Model.EnumerationModel();
                    
                    item.Name = name.ToString();
                    item.Value = Enum.Parse(typeof(T), item.Name).ToConvert<int>();
                    
                    var check = enumType.GetMember(item.Name).SingleOrDefault().GetCustomAttributes(typeof(DetailAttribute), false).SingleOrDefault();
                    item.Detail = check == null ? string.Empty : (check as DetailAttribute).Value;

                    retVal.Add(item);
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("EnumHelper.ConvertToList<>()", "An error occured while enum converting to list.", e);
                exception.Data.Add("T", typeof(T).FullName);
                throw exception;
            }

            return retVal.OrderBy( q => q.Value).ToList();
        }

        public static bool HasFlags(Type type)
        {
            System.Diagnostics.Contracts.Contract.Assert(type != null);

            Type checkedType = Nullable.GetUnderlyingType(type) ?? type;
            return HasFlagsInternal(checkedType);
        }

        private static bool HasFlagsInternal(Type type)
        {
            System.Diagnostics.Contracts.Contract.Assert(type != null);

            return type.GetCustomAttributes(false).Any(q => q is FlagsAttribute);
        }
    }
}
