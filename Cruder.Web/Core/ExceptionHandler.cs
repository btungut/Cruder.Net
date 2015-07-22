using Cruder.Core;
using Cruder.Core.Configuration;
using Cruder.Core.ExceptionHandling;
using Cruder.Core.Module;
using System;
using System.Web;

namespace Cruder.Web
{
    internal static class ExceptionHandler
    {
        internal static void HandleLastError()
        {
            string redirectUrl = ConfigurationFactory.Error.GeneralErrorPage;
            string querystring = string.Empty;

            var context = HttpContext.Current;

            if (context != null)
            {
                Exception exception = context.Server.GetLastError();

                if (exception != null)
                {
                    bool isBaseException = exception is BaseException;
                    bool isHttpException = exception is HttpException;

                    if (!Definition.IsDevelopmentEnvironment)
                    {
                        #region Wrap Exception

                        string logDescription = string.Empty;

                        if (isBaseException)
                        {
                            logDescription = ((BaseException)exception).Place;
                        }
                        else if (isHttpException)
                        {
                            int httpCode = ((HttpException)exception).GetHttpCode();
                            logDescription = string.Format("Http Exception (Code : {0})", httpCode);

                            if (httpCode == 404)
                            {
                                redirectUrl = ConfigurationFactory.Error.NotFoundPage;
                                querystring += "&path=" + context.Request.RawUrl;
                                if (context.Request.UrlReferrer != null)
                                {
                                    querystring += "&returnUrl=" + context.Request.UrlReferrer.PathAndQuery;
                                }

                            }
                            else
                            {
                                querystring += "httpCode=" + httpCode;
                            }
                        }
                        else
                        {
                            logDescription = "Unhandled Exception";
                        }

                        #endregion

                        #region Logging

                        var log = Logger.Log(LogType.Error, Priority.Normal, logDescription, exception, null, LogModule.ExceptionHandler);
                        querystring += "&logId=" + log.Data;

                        #endregion

                        #region Redirect

                        redirectUrl += "?" + querystring;
                        context.Response.Redirect(redirectUrl);

                        #endregion
                    }
                }


            }
        }
    }
}
