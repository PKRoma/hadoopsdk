namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;

    /// <summary>
    /// Provides hard coded values to ensure type safety when interacting with key static values.
    /// </summary>
    public static class HDInsightRestHardcodes
    {
        /// <summary>
        /// The X-ms-version Http Header.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "XMs", Justification = "Used to denote x-ms correct in this instance. [tgs]")]
        public static readonly KeyValuePair<string, string> XMsVersion = new KeyValuePair<string, string>("x-ms-version", "2012-08-01");

        /// <summary>
        /// The X-ms-version Http Header.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "XMs", Justification = "Used to denote x-ms correct in this instance. [tgs]")]
        public static readonly KeyValuePair<string, string> AsvXMsVersion = new KeyValuePair<string, string>("x-ms-version", "2011-08-18");

        /// <summary>
        /// The X-ms-version Http Header.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "XMs", Justification = "Used to denote x-ms correct in this instance. [tgs]")]
        public static readonly string XMsDate = "x-ms-date";

        /// <summary>
        /// The Accept Http Header.
        /// </summary>
        public static readonly KeyValuePair<string, string> Accept = new KeyValuePair<string, string>("accept", HttpHardcodes.ApplicationXml);
    }
}
