namespace Microsoft.WindowsAzure.Management.Framework.DynamicXml.Writer.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class DynaXmlElement : DynaXmlNode
    {
        public IEnumerable<DynaXmlAttribute> Attributes
        {
            get
            {
                return (from i in this.Items
                         let a = i as DynaXmlAttribute
                       where a.IsNotNull()
                      select a).ToList();
            }
        }

        public override DynaXmlNodeType NodeType
        {
            get { return DynaXmlNodeType.Element; }
        }
    }
}
