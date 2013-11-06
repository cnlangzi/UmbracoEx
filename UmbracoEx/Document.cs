using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using umbraco.NodeFactory;
using EBA.Ex;
using umbraco.interfaces;

namespace UmbracoEx
{
    public class Document
    {
        public int NodeId { get; set; }

        public int NodeTypeId { get; set; }

        public string NodeTypeAlias { get; set; }

        public bool IsTrashed { get; set; }

        public int SortOrder { get; set; }

        public string Name { get; set; }

        public string ContentXml { get; set; }

        XElement _ContextXElement = null;
        XElement ContextXElement
        {
            get
            {
                if (this._ContextXElement == null)
                {
                    this._ContextXElement = XElement.Parse(this.ContentXml);
                }

                return this._ContextXElement;
            }
        }

        public XElement GetProperty(string alias)
        {
            return this.ContextXElement.Elements().Where(i => i.Name == alias).FirstOrDefault();
        }

        public IEnumerable<Property> GetProperties()
        {
            return this.ContextXElement.Elements()
                .Select(i => new Property
                {
                    Name = i.Name.LocalName,
                    Value = i.Value
                });
        }

        public T GetPropertyValue<T>(string alias)
        {
            var el = this.GetProperty(alias);
            return el == null ? default(T) : el.Value.ConvertTo<T>();
        }

        public T GetPropertyValue<T>(string alias, T defaultValue)
        {
            var el = this.GetProperty(alias);
            return el == null ? defaultValue : el.Value.ConvertTo<T>(defaultValue);
        }

        public Nullable<T> GetPropertyNullableValue<T>(string alias) where T : struct
        {
            var el = this.GetProperty(alias);
            return el == null ? null : el.Value.ConvertToNullable<T>();
        }


        public INode ToNode()
        {
            return new Node(this.NodeId);
        }
    }
}
