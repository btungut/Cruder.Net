using System.Text;
using System.Web.Mvc;

namespace Cruder.Helper
{
    public static class HtmlExtension
    {
        public static CruderHtmlHelper<TModel> Cruder<TModel>(this HtmlHelper<TModel> html)
        {
            return new CruderHtmlHelper<TModel>(html);
        }

        public static MvcHtmlString Validate(this MvcHtmlString html)
        {
            return AppendHtmlTag(html, "class", "validate[required]");
        }

        public static MvcHtmlString Validate(this MvcHtmlString html, string custom)
        {
            return AppendHtmlTag(html, "class", "validate["+custom+"]");
        }

        public static MvcHtmlString AppendHtmlTag(this MvcHtmlString html, string tagName, string value)
        {
            StringBuilder builder = new StringBuilder();

            string htmlstring = html.ToHtmlString();

            if (htmlstring.Contains(tagName))
            {
                int startIndex = htmlstring.IndexOf(tagName);
                int closingQuotationMarkIndex = htmlstring.IndexOf('"', startIndex + tagName.Length + 2);

                builder.Append(htmlstring.Substring(0, closingQuotationMarkIndex));
                builder.Append(" " + value);
                builder.Append(htmlstring.Substring(closingQuotationMarkIndex));
            }
            else
            {
                int endTagIndex = htmlstring.EndsWith("/>") ? htmlstring.Length - 2 : htmlstring.Length - 1;

                builder.Append(htmlstring.Substring(0, endTagIndex));
                builder.Append("" + tagName + "=\"" + value + "\" ");
                builder.Append(htmlstring.Substring(endTagIndex));
            }
            
            return new MvcHtmlString(builder.ToString());
        }
    }
}
