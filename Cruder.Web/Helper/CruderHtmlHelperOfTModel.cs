using System.Web.Mvc;

namespace Cruder.Helper
{
    public class CruderHtmlHelper<TModel> : CruderHtmlHelper
    {
        public HtmlHelper<TModel> HtmlHelper { get; private set; }

        public CruderHtmlHelper(HtmlHelper<TModel> htmlHelper):base(htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }
    }
}
