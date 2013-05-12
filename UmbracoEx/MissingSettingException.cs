using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UmbracoEx
{
    [Serializable]
    public class MissingSettingException : Exception
    {
        public MissingSettingException() { }
        public MissingSettingException(string message) : base(message) { }
        public MissingSettingException(string message, Exception inner) : base(message, inner) { }
        protected MissingSettingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
