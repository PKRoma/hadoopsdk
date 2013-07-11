// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.WindowsAzure.Management.Framework.DynamicXml.Reader
{
    /// <summary>
    /// Represents a mapping of a namespace prefix to its namespace uri.
    /// </summary>
    public class DynaXmlNamespaceMapping
    {
        /// <summary>
        ///    Initializes a new instance of the DynaXmlNamespaceMapping class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="xmlNamespace">The URI.</param>
        public DynaXmlNamespaceMapping(string prefix, string xmlNamespace)
        {
            this.Prefix = prefix;
            this.XmlNamespace = xmlNamespace;
        }

        /// <summary>
        /// Gets the xmlNamespace for this mapping.
        /// </summary>
        public string XmlNamespace { get; private set; }

        /// <summary>
        /// Gets the prefix for this mapping.
        /// </summary>
        public string Prefix { get; private set; }
    }
}
