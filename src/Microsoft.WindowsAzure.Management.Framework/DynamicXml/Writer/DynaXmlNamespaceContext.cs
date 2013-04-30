namespace Microsoft.WindowsAzure.Management.Framework.DynamicXml.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class DynaXmlNamespaceContext
    {
        /// <summary>
        /// Initializes a new instance of the DynaXmlNamespaceContext class.
        /// </summary>
        /// <param name="orignal">
        /// An existing namespace context that should be cloned.
        /// </param>
        internal DynaXmlNamespaceContext(DynaXmlNamespaceContext orignal)
        {
            this.CurrentAlias = orignal.CurrentAlias;
            this.DefaultNamespace = orignal.DefaultNamespace;
            this.AliasTable = new Dictionary<string, string>(orignal.AliasTable);
        }

        /// <summary>
        /// Initializes a new instance of the DynaXmlNamespaceContext class.
        /// </summary>
        internal DynaXmlNamespaceContext()
        {
            this.CurrentAlias = string.Empty;
            this.DefaultNamespace = string.Empty;
            this.AliasTable = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the default namespace.
        /// </summary>
        public string DefaultNamespace { get; set; }

        /// <summary>
        /// Gets or sets the current alias.
        /// </summary>
        public string CurrentAlias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current alias should be applied to attributes.
        /// </summary>
        public bool ApplyCurrentToAttributes { get; set; }

        /// <summary>
        /// Gets the current namespace table (organized by alias).
        /// </summary>
        public IDictionary<string, string> AliasTable { get; private set; }
    }
}
