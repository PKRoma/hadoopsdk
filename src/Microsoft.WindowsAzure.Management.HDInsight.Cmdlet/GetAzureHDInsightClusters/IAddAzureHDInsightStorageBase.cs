namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IAddAzureHDInsightStorageBase
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
    }
}
