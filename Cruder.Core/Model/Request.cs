using System;
using System.Collections.Generic;
using System.Linq;

namespace Cruder.Core.Model
{
    public sealed class Request
    {
        public string HttpMethod { get; set; }

        public string Url { get; set; }

        public string PhysicalPath { get; set; }

        public IDictionary<string, string> RouteValues { get; set; }

        public IDictionary<string, string> Forms { get; set; }

        public IDictionary<string, string> Cookies { get; set; }

        public IDictionary<string, string> Params { get; set; }

        public static Request GetCurrent()
        {
            Request retVal = null;

            try
            {
                var context = System.Web.HttpContext.Current;
                if (context != null)
                {
                    retVal = new Request();

                    if (!string.IsNullOrEmpty(context.Request.HttpMethod))
                    {
                        retVal.HttpMethod = context.Request.HttpMethod;
                    }

                    if (!string.IsNullOrEmpty(context.Request.PhysicalPath))
                    {
                        retVal.PhysicalPath = context.Request.PhysicalPath;
                    }

                    if (!string.IsNullOrEmpty(context.Request.Url.OriginalString))
                    {
                        retVal.Url = context.Request.Url.OriginalString;
                    }

                    if (context.Request.RequestContext != null && context.Request.RequestContext.RouteData != null)
                    {
                        retVal.RouteValues = context.Request.RequestContext.RouteData.Values.ToDictionary(q => q.Key, q => q.Value.ToString());
                    }
                    
                    if (context.Request.Form != null && context.Request.Form.Count > 0)
                    {
                        retVal.Forms = new Dictionary<string, string>();
                        context.Request.Form.AllKeys.AsParallel().ForAll((key) =>
                            {
                                retVal.Forms.Add(key, context.Request.Form.Get(key));
                            });
                    }
                    
                    if (context.Request.Cookies != null && context.Request.Cookies.Count > 0)
                    {
                        retVal.Cookies = new Dictionary<string, string>();
                        foreach (var item in context.Request.Cookies.AllKeys)
                        {
                            retVal.Cookies.Add(item, context.Request.Cookies.Get(item).Value);
                        }
                    }

                    if (context.Request.Params != null && context.Request.Params.Count > 0)
                    {
                        string[] wantedKeys = { "AUTH_TYPE", "AUTH_USER", "LOGON_USER", "REMOTE_USER"};
                        retVal.Params = new Dictionary<string, string>();

                        foreach (var item in wantedKeys)
                        {
                            retVal.Params.Add(item, context.Request.Params.Get(item));
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return retVal;
        }
    }
}
