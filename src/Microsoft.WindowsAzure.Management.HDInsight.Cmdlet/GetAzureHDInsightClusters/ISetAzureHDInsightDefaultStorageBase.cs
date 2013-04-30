namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a command to set a default storage account for an Azure HDInsight config.
    /// </summary>
    internal interface ISetAzureHDInsightDefaultStorageBase
    {
        /// <summary>
        /// Gets or sets the AzureHDInsightConfig.
        /// </summary>
        AzureHDInsightConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the Storage Account Name.
        /// </summary>
        string StorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the Storage Account Key.
        /// </summary>
        string StorageAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the Storage Container Name.
        /// </summary>
        string StorageContainerName { get; set; }
    }
}
