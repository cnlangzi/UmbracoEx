using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UmbracoEx.Web
{
    public class AjaxPostMethodHandler
    {
        public Predicate<string> Match { get; set; }
        public Func<object> ExecuteMethod { get; set; }
    }
}
