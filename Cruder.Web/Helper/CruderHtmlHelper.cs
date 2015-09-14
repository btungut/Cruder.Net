using Cruder.Core.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Cruder.Web;
using Cruder.Web.ViewModel;

namespace Cruder.Helper
{
    public class CruderHtmlHelper
    {
        internal const string RequiredMarkString = "<span class=\"requiredStar\">*</span>";

        public HtmlHelper HtmlHelper { get; private set; }

        public CruderHtmlHelper(HtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }

        internal bool IsMemberRequired(MemberInfo memberInfo)
        {
            bool retVal = false;

            if (memberInfo.GetCustomAttributes(true).Any(q => q is RequiredAttribute))
            {
                retVal = true;
            }
            else if (true)
            {
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                FieldInfo fieldInfo = memberInfo as FieldInfo;

                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType.IsValueType &&
                    !(propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        retVal = true;
                    }
                }
                else if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType.IsValueType &&
                    !(fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        internal string GenerateValidationEngineClass(MemberInfo memberInfo)
        {
            List<string> validationAttributes = new List<string>();

            object[] customAttributes = memberInfo.GetCustomAttributes(true);

            if (IsMemberRequired(memberInfo))
            {
                validationAttributes.Add("required");
            }

            object withClassAttribute = customAttributes.SingleOrDefault(attribute => attribute is ValidationEngineAttribute);
            if (withClassAttribute != null)
            {
                validationAttributes.Add((withClassAttribute as ValidationEngineAttribute).ValidationClass);
            }
            else if (customAttributes.Any())
            {
                foreach (object attribute in customAttributes)
                {
                    if (attribute is RangeAttribute)
                    {
                        validationAttributes.Add(string.Format("min[{0}]", (attribute as RangeAttribute).Minimum));
                        validationAttributes.Add(string.Format("max[{0}]", (attribute as RangeAttribute).Maximum));
                    }
                    else if (attribute is MinLengthAttribute)
                    {
                        validationAttributes.Add(string.Format("minSize[{0}]", (attribute as MinLengthAttribute).Length));
                    }
                    else if (attribute is MaxLengthAttribute)
                    {
                        validationAttributes.Add(string.Format("maxSize[{0}]", (attribute as MaxLengthAttribute).Length));
                    }
                    else if (attribute is StringLengthAttribute)
                    {
                        validationAttributes.Add(string.Format("minSize[{0}]", (attribute as StringLengthAttribute).MinimumLength));
                        validationAttributes.Add(string.Format("maxSize[{0}]", (attribute as StringLengthAttribute).MaximumLength));
                    }
                }
            }

            StringBuilder retVal = new StringBuilder("validate[");
            validationAttributes.ForEach(attribute => retVal.Append(attribute + ","));

            return retVal.Remove(retVal.Length - 1, 1).Append("]").ToString();
        }

        public MvcHtmlString RequiredMark()
        {
            return MvcHtmlString.Create(RequiredMarkString);
        }

        public IEnumerable<SelectListItem> FormatableSelectList(System.Collections.IEnumerable items, string dataValueFormat, string dataTextFormat)
        {
            List<SelectListItem> retVal = new List<SelectListItem>();

            foreach (var item in items)
            {
                string value = item.ToStringWithFormat(dataValueFormat);
                string text = item.ToStringWithFormat(dataTextFormat);

                SelectListItem listItem = new SelectListItem
                {
                    Value = value,
                    Text = text
                };

                retVal.Add(listItem);
            }

            return retVal;
        }

        public MvcHtmlString ColumnSortLink(string memberName)
        {
            string retVal = "{0}&OrderBy={1}&OrderType={2}";

            var nameValueCollection = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());
            nameValueCollection.Remove("OrderBy");
            nameValueCollection.Remove("OrderType");

            var request = HtmlHelper.ViewContext.HttpContext.Request;
            string currentOrderBy = request.QueryString["OrderBy"];
            string currentOrderType = request.QueryString["OrderType"];
            string url = request.Path + "?" + nameValueCollection;

            if (currentOrderBy != null && currentOrderBy.ToLowerInvariant().Equals(memberName.ToLowerInvariant()) && currentOrderType != null && currentOrderType.ToLowerInvariant().Equals("asc"))
            {
                retVal = string.Format(retVal, url, memberName, "desc");
            }
            else
            {
                retVal = string.Format(retVal, url, memberName, "asc");
            }

            return MvcHtmlString.Create(retVal);
        }

        public List<SelectListItem> BooleanSelectList(string optionText = "Select", string trueText = "Yes", string falseText = "No")
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = optionText, Value=string.Empty},
                new SelectListItem{ Text = trueText, Value="true"},
                new SelectListItem{ Text = falseText, Value="false"}
            };
        }

        public List<SelectListItem> CriteriaOptions(
            string equalsText = "Equals",
            string notEqualsText = "Not Equals",
            string greaterText = "Greater",
            string smallerText = "Smaller",
            string containsText = "Contains",
            string notContainsText = "Not Contains")
        {
            List<SelectListItem> retVal = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(equalsText))
                retVal.Add(new SelectListItem { Text = equalsText, Value = CriteriaOptionEnum.Equals.ToString() });
            if (!string.IsNullOrEmpty(notEqualsText))
                retVal.Add(new SelectListItem { Text = notEqualsText, Value = CriteriaOptionEnum.NotEquals.ToString() });
            if (!string.IsNullOrEmpty(greaterText))
                retVal.Add(new SelectListItem { Text = greaterText, Value = CriteriaOptionEnum.Greater.ToString() });
            if (!string.IsNullOrEmpty(smallerText))
                retVal.Add(new SelectListItem { Text = smallerText, Value = CriteriaOptionEnum.Smaller.ToString() });
            if (!string.IsNullOrEmpty(containsText))
                retVal.Add(new SelectListItem { Text = containsText, Value = CriteriaOptionEnum.Contains.ToString() });
            if (!string.IsNullOrEmpty(notContainsText))
                retVal.Add(new SelectListItem { Text = notContainsText, Value = CriteriaOptionEnum.NotContains.ToString() });

            return retVal;
        }

        public List<SelectListItem> CriteriaOptionsForDecimal(string equalsText = "Equals", string notEqualsText = "Not Equals", string greaterText = "Greater", string smallerText = "Smaller")
        {
            return this.CriteriaOptions(equalsText: equalsText, notEqualsText: notEqualsText, greaterText: greaterText, smallerText: smallerText, containsText: null, notContainsText: null);
        }

        public List<SelectListItem> CriteriaOptionsForString(string equalsText = "Equals", string notEqualsText = "Not Equals", string containsText = "Contains", string notContainsText = "Not Contains")
        {
            return this.CriteriaOptions(equalsText: equalsText, notEqualsText: notEqualsText, greaterText: null, smallerText: null, containsText: containsText, notContainsText: notContainsText);
        }

        public List<SelectListItem> CriteriaOptionsForBoolean(string equalsText = "Equals", string notEqualsText = "Not Equals")
        {
            return this.CriteriaOptions(equalsText: equalsText, notEqualsText: notEqualsText, greaterText: null, smallerText: null, containsText: null, notContainsText: null);
        }

        public MessageModel PageMessage
        {
            get
            {
                MessageModel retVal = null;

                var data = HtmlHelper.ViewContext.TempData[Constants.PageMessageKey];

                if (data != null)
                {
                    retVal = (MessageModel)data;
                }

                return retVal;
            }
        }
    }
}
