using Cruder.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruder.Web
{
    public class CruderWebViewPage : System.Web.Mvc.WebViewPage
    {
        public CruderHtmlHelper CruderHtml { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            CruderHtml = new CruderHtmlHelper(base.Html);
        }

        public override void Execute()
        {
        }
    }

    public class CruderWebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public CruderHtmlHelper<TModel> CruderHtml { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            CruderHtml = new CruderHtmlHelper<TModel>(base.Html);
        }

        public override void Execute()
        {
        }
    }
}
