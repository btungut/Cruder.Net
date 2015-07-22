using Cruder.Core.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cruder.Core.Repository
{
    public sealed class QueryCriterias : IEnumerable<QueryCriteriaItem>
    {
        private readonly List<QueryCriteriaItem> list = null;

        public QueryCriterias()
        {
            this.list = new List<QueryCriteriaItem>();
        }

        public QueryCriteriaItem this[int index]
        {
            get { return this.ElementAt(index); }
        }

        public IEnumerator<QueryCriteriaItem> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Insert(QueryCriteriaItem queryCriteria)
        {
            if (queryCriteria == null) throw new ArgumentNullException("queryCriteria");
            if (list.Any(q => q.Key == queryCriteria.Key)) throw new ArgumentException("An item with the same key has already been added.");

            list.Add(queryCriteria);
        }

        public void Add(QueryCriteriaItem queryCriteria)
        {
            Insert(queryCriteria);
        }

        public void Add(string key, string value)
        {
            Insert(new QueryCriteriaItem { Key = key, Value = value });
        }

        public void Add(string key, string value, CriteriaOptionEnum option)
        {
            Insert(new QueryCriteriaItem { Key = key, Value = value, Option = option });
        }

        public static QueryCriterias Parse(string queryString)
        {
            try
            {
                QueryCriterias retVal = new QueryCriterias();

                List<string> searchList = queryString.Split('&').Where(q => q.Contains("Search.")).ToList();
                List<string> optionList = queryString.Split('&').Where(q => q.Contains("Option.")).ToList();

                foreach (var item in searchList)
                {
                    string query = item.Replace("Search.", string.Empty);
                    string key = query.Substring(0, query.IndexOf('='));
                    string value = query.Substring(query.IndexOf('=') + 1, query.Length - query.IndexOf('=') - 1);

                    if (string.IsNullOrEmpty(value) || retVal.Any(q => q.Key.Equals(key)))
                        continue;

                    value = value.Contains("true,false") ? "true" : value;

                    QueryCriteriaItem criteria = new QueryCriteriaItem();
                    criteria.Key = key;
                    criteria.Value = value;
                    criteria.Option = CriteriaOptionEnum.Equals; //defaul value

                    if (optionList.Any(q => q.StartsWith("Option." + key + "=")))
                    {
                        var optionElement = optionList.Single(q => q.StartsWith("Option." + key + "="));
                        var optionValue = optionElement.Substring(optionElement.IndexOf('=') + 1, optionElement.Length - optionElement.IndexOf('=') - 1).ToLowerInvariant();

                        if (optionValue == "equals")
                            criteria.Option = CriteriaOptionEnum.Equals;
                        else if (optionValue == "notequals")
                            criteria.Option = CriteriaOptionEnum.NotEquals;
                        else if (optionValue == "greater")
                            criteria.Option = CriteriaOptionEnum.Greater;
                        else if (optionValue == "smaller")
                            criteria.Option = CriteriaOptionEnum.Smaller;
                        else if (optionValue == "contains")
                            criteria.Option = CriteriaOptionEnum.Contains;
                        else if (optionValue == "notcontains")
                            criteria.Option = CriteriaOptionEnum.NotContains;
                    }

                    retVal.Add(criteria);
                }

                return retVal;
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("QueryCriterias.Parse()", "An error occurred while parsing QueryCriterias.", e);
                exception.Data.Add("queryString", queryString);
                throw exception;
            }
        }
    }

    public sealed class QueryCriteriaItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public CriteriaOptionEnum Option { get; set; }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            QueryCriteriaItem item = obj as QueryCriteriaItem;

            if (item != null &&
                item.Key == this.Key &&
                item.Option == this.Option &&
                item.Value == this.Value)
            {
                retVal = true;
            }

            return retVal;
        }
    }

    [DefaultValue("Equals")]
    public enum CriteriaOptionEnum
    {
        Equals,
        NotEquals,
        Greater,
        Smaller,
        Contains,
        NotContains
    }
}
