using System.Collections.Generic;
using System.Web;
using System.Linq;


namespace UmbracoEx.Web
{
    /// <summary>
    /// Manage all client scripts, prevent to load one script twices.
    /// </summary>
    public class ScriptContext
    {
        private ScriptContext()
        {
        }

        List<ScriptVariable> _Variables = new List< ScriptVariable>();
        List<ScriptFile> _Scripts = new List<ScriptFile>();



        public ScriptContext AddVariable(string name, string value)
        {
            this._Variables.Add(new ScriptVariable { Name = name, Value = value });
            return this;
        }

        public ScriptContext Include(string name, string origin, string match="true", string minified=null)
        {
            var lowerName = name.ToLowerInvariant();
            if (this._Scripts.Any(i=>i.Name == lowerName) == false)
            {
                this._Scripts.Add(new ScriptFile { Name = lowerName, Match=match, Source = origin, Minified= minified??origin });
            }

            return this;
        }


        public IEnumerable<ScriptFile> GetScripts()
        {
            return this._Scripts;
        }

        public IEnumerable<ScriptVariable> GetVariables()
        {
            return this._Variables;
        }

        static object SCRIPTCONTEXT = new object();

        public static ScriptContext Current
        {
            get
            {
                var currentRequest = HttpContext.Current;
                var context = currentRequest.Items[SCRIPTCONTEXT] as ScriptContext;
                if (context == null)
                {
                    context = new ScriptContext();
                    currentRequest.Items[SCRIPTCONTEXT] = context;
                }

                return context;
            }
        }
    }
}