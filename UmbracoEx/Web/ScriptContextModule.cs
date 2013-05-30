using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using EBA.Helpers;

[assembly: WebResource("UmbracoEx.Web.UEX.js", "text/javascript", PerformSubstitution = true)]
namespace UmbracoEx.Web
{

    public class ScriptContextModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
        }

        void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            var page = HttpContext.Current.CurrentHandler as Page;
            if (page != null)
            {
                page.PreRender += new EventHandler(page_PreRender);
            }
        }

        void page_PreRender(object sender, EventArgs e)
        {
            var page = HttpContext.Current.CurrentHandler as Page;
            if (page != null && page.Request != null && page.Request["AjaxPostMethod"] == null)
            {
                var scriptContext = ScriptContext.Current;


                page.ClientScript.RegisterClientScriptInclude("UEX.js", page.ClientScript.GetWebResourceUrl(typeof(ScriptContext), "UmbracoEx.Web.UEX.js"));

                //ScriptManager.RegisterClientScriptInclude(page, this.GetType(), "UEX.js", "UmbracoEx.Web.UEX.js");


                var minified = System.Configuration.ConfigurationManager.AppSettings["jsminify"] == "true";

                var variables = string.Join(";", scriptContext.GetVariables()
                    .Select(i => i.Value.StartsWith("var " + i.Name) ? i.Value : "var " + i.Name + "=" + i.Value)
                    .ToArray());

               

                if (variables.HasValue())
                {
                    page.ClientScript.RegisterStartupScript(typeof(ScriptVariable), "scriptVariables", "<script type=\"text/javascript\">" + variables + "</script>");
                }

                var scripts = string.Join("", scriptContext.GetScripts().Select(i => string.Format("UEX.require({0},'{1}');", i.Match, GetScriptName(page, minified ? i.Minified : i.Source))));

                if (scripts.HasValue())
                {


                    page.ClientScript.RegisterStartupScript(typeof(ScriptFile), "scriptFiles", scripts, true);
                }
            }

        }

        private string GetScriptName(Page page, string url)
        {
            if (url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                return url;
            }
            else
            {
                try
                {

                    var fileName = page.Server.MapPath(url);

                    if (File.Exists(fileName))
                    {
                        return url + "?v=" + new FileInfo(fileName).LastWriteTimeUtc.Ticks.ToString();
                    }
                    else
                    {
                        return url;
                    }
                }
                catch
                {
                    return url;
                }
            }

        }
    }

}