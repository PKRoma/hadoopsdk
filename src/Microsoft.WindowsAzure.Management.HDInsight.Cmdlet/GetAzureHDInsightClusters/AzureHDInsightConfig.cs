namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents an Azure Configuration to be used when creating new clusters.
    /// </summary>
    public class AzureHDInsightConfig
    {
        /// <summary>
        /// Initializes a new instance of the AzureHDInsightConfig class.
        /// </summary>
        public AzureHDInsightConfig()
        {
            this.DefaultStorageAccount = new AzureHDInsightDefaultStorageAccount();
            this.AdditionalStorageAccounts = new List<AzureHDInsightStorageAccount>();
        }

        /// <summary>
        /// Gets or sets the size of the cluster in data nodes.
        /// </summary>
        public int ClusterSizeInNodes { get; set; }

        /// <summary>
        /// Gets or sets the default storage account for the HDInsight cluster.
        /// </summary>
        public AzureHDInsightDefaultStorageAccount DefaultStorageAccount { get; set; }

        /// <summary>
        /// Gets the additional storage accounts for the HDInsight cluster.
        /// </summary>
        public ICollection<AzureHDInsightStorageAccount> AdditionalStorageAccounts { get; private set; }

        /// <summary>
        /// Gets or sets the Hive Metastore.
        /// </summary>
        public AzureHDInsightMetastore HiveMetastore { get; set; }

        /// <summary>
        /// Gets or sets the Oozie Metastore.
        /// </summary>
        public AzureHDInsightMetastore OozieMetastore { get; set; }
    }
}
