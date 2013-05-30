using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UmbracoEx
{
    public static class UrlHelper
    {
        public static string MakeAbsoluteUrl(HttpRequest request, string relativeUrl, bool includeScheme = true)
        {
            if (includeScheme)
            {
                return string.Format("{0}://{1}{2}{3}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath.TrimEnd('/'), relativeUrl);
            }
            else
            {
                return string.Format("{0}{1}{2}", request.Url.Authority, request.ApplicationPath.TrimEnd('/'), relativeUrl);
            }
        }

    }
}
