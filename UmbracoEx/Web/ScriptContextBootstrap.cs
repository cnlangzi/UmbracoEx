using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivator.PreApplicationStartMethodAttribute(typeof(UmbracoEx.Web.ScriptContextBootstrap), "Initialize")]
namespace UmbracoEx.Web
{
    public static class ScriptContextBootstrap
    {
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(ScriptContextModule));
        }
    }
}