using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cruder.Helper
{
    public static class CruderHtmlExtension
    {
        public static MvcHtmlString RequiredMarkFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            string retVal = string.Empty;

            bool isRequried = cruderHtmlHelper.IsMemberRequired((expression.Body as MemberExpression).Member);

            if (isRequried)
            {
                retVal = CruderHtmlHelper.RequiredMarkString;
            }

            return MvcHtmlString.Create(retVal);
        }

        public static MvcHtmlString ValidationClassFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(cruderHtmlHelper.GenerateValidationEngineClass((expression.Body as MemberExpression).Member));
        }
    }
}
