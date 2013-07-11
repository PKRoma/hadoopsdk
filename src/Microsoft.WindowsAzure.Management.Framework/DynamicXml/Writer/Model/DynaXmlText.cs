namespace Microsoft.WindowsAzure.Management.Framework.DynamicXml.Writer.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class DynaXmlText : DynaXmlNode
    {
        public override DynaXmlNodeType NodeType
        {
            get { return DynaXmlNodeType.Text; }
        }
    }
}
