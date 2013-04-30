namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Adds a storage account to the HDInsight cluster configuration.
    /// </summary>
    internal class AddAzureHDInsightStorageCommand : IAddAzureHDInsightStorageCommand
    {
        /// <summary>
        /// Initializes a new instance of the AddAzureHDInsightStorageCommand class.
        /// </summary>
        public AddAzureHDInsightStorageCommand()
        {
            this.Config = new AzureHDInsightConfig();
            this.Output = new Collection<AzureHDInsightConfig>();
        }

        public void EndProcessing()
        {
            var account = new AzureHDInsightStorageAccount();
            account.StorageAccountName = this.StorageAccountName;
            account.StorageAccountKey = this.StorageAccountKey;
            this.Config.AdditionalStorageAccounts.Add(account);
            this.Output.Add(this.Config);
        }

        public ICollection<AzureHDInsightConfig> Output { get; private set; }

        public AzureHDInsightConfig Config { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }
    }
}
