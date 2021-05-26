using Quartz;
using Sentry;
using Sentry.Protocol;
using System;
using System.Threading;
using System.Web;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    public static class JobHelper
    {
        public static string GetQueryString(string apiUrl, string paramName)
        {
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {                
                var uri = new Uri(apiUrl);

                return HttpUtility.ParseQueryString(uri.Query).Get(paramName);

            }

            return string.Empty;
        }

        public static int GetQueryInt(string apiUrl, string paramName)
        {
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                var uri = new Uri(apiUrl);

                return Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get(paramName));

            }

            return 0;
        }

        public static string GetUrl(string apiUrl)
        {
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                var uri = new Uri(apiUrl);

                return string.IsNullOrWhiteSpace(uri.Query) ? apiUrl : uri.AbsoluteUri.Replace(uri.Query, string.Empty);

            }

            return string.Empty;
        }

        public static JobExecutionException JobExecuteFailed(string errorMessage, ISentryClient sentryClient)
        {
            var jobExecutionException = new JobExecutionException(errorMessage);

            sentryClient.CaptureMessage(errorMessage, SentryLevel.Error);

            Thread.Sleep(60000);

            jobExecutionException.RefireImmediately = true;

            return jobExecutionException;
        }
    }
}
