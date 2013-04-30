namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class SetAzureHDInsightDefaultStorageCommand : AzureHDInsightCommand<AzureHDInsightConfig>, ISetAzureHDInsightDefaultStorageCommand
    {
        public SetAzureHDInsightDefaultStorageCommand()
        {
            this.Config = new AzureHDInsightConfig();
        }

        public override void EndProcessing()
        {
            this.Config.DefaultStorageAccount.StorageAccountName = this.StorageAccountName;
            this.Config.DefaultStorageAccount.StorageAccountKey = this.StorageAccountKey;
            this.Config.DefaultStorageAccount.StorageContainerName = this.StorageContainerName;
            this.Output.Add(this.Config);
        }

        public AzureHDInsightConfig Config { get; set; }
        
        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }

        public string StorageContainerName { get; set; }
    }
}
