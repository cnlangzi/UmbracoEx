using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using EBA.Helpers;
using System.Web.Script.Serialization;
using System.Web;
using System.Threading;

namespace UmbracoEx.Web
{
    public class AjaxPostRequest
    {
        public static void Process<T>(T target, params AjaxPostMethodHandler[] handlers)
        {

            var context = HttpContext.Current;
            var methodName = context.Request["AjaxPostMethod"];
            if (methodName.HasValue())
            {

                Func<object> func = null;

                var handler = handlers == null ? null : handlers.Where(i => i.Match(methodName)).FirstOrDefault();

                if (handler != null)
                {
                    func = () => handler.ExecuteMethod();
                }
                else
                {
                    var method = typeof(T).GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (method != null)
                    {
                        func = () => method.Invoke(target, new object[] { });
                    }
                }


                if (func != null)
                {

                    var response = context.Response;

                    var serializer = new JavaScriptSerializer();
                   
                    response.ClearContent();
                    response.ContentType = "application/json";


                    try
                    {
                        object result = func();
                        response.Write(serializer.Serialize(result));
                    }
                    catch (ThreadAbortException)//http://support.microsoft.com/kb/312629
                    {
                        context.Server.ClearError();
                    }
                    catch (Exception ex)
                    {
                        response.Write(serializer.Serialize(new { Error = ex.InnerException==null? ex.Message:ex.InnerException.Message }));
                    }

                    response.End();
                }
            }
        }
    }
}
