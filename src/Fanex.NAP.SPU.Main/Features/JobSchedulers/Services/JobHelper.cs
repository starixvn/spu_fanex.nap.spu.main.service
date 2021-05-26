using System;
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
    }
}
