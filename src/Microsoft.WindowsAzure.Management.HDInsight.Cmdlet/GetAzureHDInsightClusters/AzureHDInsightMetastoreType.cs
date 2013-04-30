namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines the type of an AzureHDInsight cluster metastore.
    /// </summary>
    public enum AzureHDInsightMetastoreType
    {
        /// <summary>
        /// A Hive metastore.
        /// </summary>
        HiveMetastore,

        /// <summary>
        /// An Oozie metastore.
        /// </summary>
        OozieMetastore,
    }
}
