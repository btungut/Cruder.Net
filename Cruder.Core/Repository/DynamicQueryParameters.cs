using Cruder.Core.ExceptionHandling;
using System;
using System.Linq;

namespace Cruder.Core.Repository
{
    public sealed class DynamicQueryParameters
    {
        public string Query { get; set; }

        public object[] Values { get; set; }

        public static DynamicQueryParameters Parse(QueryCriterias queryCriterias, Type entityType)
        {
            DynamicQueryParameters retVal = new DynamicQueryParameters();

            try
            {
                retVal.Values = new object[queryCriterias.Count()];
                int counter = 0;

                for (int i = 0; i < queryCriterias.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(retVal.Query))
                    {
                        retVal.Query = retVal.Query + " AND ";
                    }

                    var iterator = queryCriterias.Skip(i).Take(1).First();
                    string key = iterator.Key;
                    string value = iterator.Value;
                    string memberTypeName = string.Empty;
                    Type memberType = entityType;

                    foreach (var item in key.Split('.'))
                    {
                        if (memberType.GetProperty(item) != null)
                        {
                            memberType = memberType.GetProperty(item).PropertyType;
                        }
                        else if (memberType.GetField(item) != null)
                        {
                            memberType = memberType.GetField(item).FieldType;
                        }
                        else
                        {
                            throw new FrameworkException("QueryCriterias.ToDynamicQuery()", string.Format("No member found for given '{0}' property/field name.", item));
                        }
                    }

                    memberTypeName = memberType.Name.ToLowerInvariant();

                    if (memberTypeName.Contains("int") ||
                        memberTypeName.Contains("double") ||
                        memberTypeName.Contains("float") ||
                        memberTypeName.Contains("decimal"))
                    {
                        if (iterator.Option == CriteriaOptionEnum.Equals)
                            retVal.Query = retVal.Query + string.Format("{0}=@{1}", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.Greater)
                            retVal.Query = retVal.Query + string.Format("{0}>@{1}", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.NotEquals)
                            retVal.Query = retVal.Query + string.Format("{0}!=@{1}", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.Smaller)
                            retVal.Query = retVal.Query + string.Format("{0}<@{1}", key, counter);

                        retVal.Values[i] = decimal.Parse(value);
                        counter++;
                    }
                    else if (memberTypeName.Contains("bool") ||
                        memberTypeName.Contains("nullable"))
                    {
                        retVal.Query = retVal.Query + string.Format("{0}={1}", key, value.ToLowerInvariant() == "null" ? "null" : value);
                        counter++;
                    }
                    else if (memberType.IsEnum)
                    {
                        if (iterator.Option == CriteriaOptionEnum.Equals)
                            retVal.Query = retVal.Query + string.Format("{0}=\"{1}\"", key, value); 
                        else if (iterator.Option == CriteriaOptionEnum.NotEquals)
                            retVal.Query = retVal.Query + string.Format("{0}!=\"{1}\"", key, value);

                        counter++;
                    }
                    else if (memberTypeName.Contains("string"))
                    {
                        if (iterator.Option == CriteriaOptionEnum.Equals)
                            retVal.Query = retVal.Query + string.Format("{0}.Equals(@{1})", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.NotEquals)
                            retVal.Query = retVal.Query + string.Format("!{0}.Equals(@{1})", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.Contains)
                            retVal.Query = retVal.Query + string.Format("{0}.Contains(@{1})", key, counter);
                        else if (iterator.Option == CriteriaOptionEnum.NotContains)
                            retVal.Query = retVal.Query + string.Format("!{0}.Contains(@{1})", key, counter);

                        retVal.Values[i] = value;
                        counter++;
                    }
                    else if (memberTypeName.Contains("guid"))
                    {
                        retVal.Query = retVal.Query + string.Format("{0}.Equals(@{1})", key, counter);
                        retVal.Values[i] = value;
                        counter++;
                    }
                    else
                    {
                        retVal.Query = retVal.Query + string.Format("{0}.Contains(@{1})", key, counter);
                        retVal.Values[i] = value;
                        counter++;
                    }
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("QueryCriterias.ToDynamicQuery()", "An error occurred while Dynamic Query Parameter parsing.", e);
                throw exception;
            }

            return retVal;
        }
    }
}
