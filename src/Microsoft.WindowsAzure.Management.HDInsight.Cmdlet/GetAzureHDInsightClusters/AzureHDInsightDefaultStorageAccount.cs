namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a Default Storage Account used for an HDInsight cluster.
    /// </summary>
    public class AzureHDInsightDefaultStorageAccount : AzureHDInsightStorageAccount
    {
        /// <summary>
        /// Gets or sets the Storage Container for the Default Storage Account.
        /// </summary>
        public string StorageContainerName { get; set; }
    }
}
