using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoEx.Web
{
    public class ScriptFile
    {
        public string Name { get; set; }
        public string Match { get; set; }
        public string Source { get; set; }
        public string Minified { get; set; }
    }
}