namespace Microsoft.WindowsAzure.Management.Framework.DynamicXml.Writer.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal enum DynaXmlNodeType
    {
        Attribute,
        CData,
        Text,
        Element,
        Document
    }

    internal abstract class DynaXmlNode
    {
        public string XmlNamespace { get; set; }
        
        public string Prefix { get; set; }
        
        public string LocalName { get; set; }

        public string Value;

        public DynaXmlNode()
        {
            this.Items = new List<DynaXmlNode>();
        }

        public ICollection<DynaXmlNode> Items { get; private set; }

        public abstract DynaXmlNodeType NodeType { get; }
    }
}
