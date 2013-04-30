namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a Storage Account for an HD Insight Configuration.
    /// </summary>
    public class AzureHDInsightStorageAccount
    {
        /// <summary>
        /// Gets or sets the Storage Account Name.
        /// </summary>
        public string StorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the Storage Account Key.
        /// </summary>
        public string StorageAccountKey { get; set; }
    }
}
