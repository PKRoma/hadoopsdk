namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Adds an AzureHDInsight Storage Account to the current configuration.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, AzureHdInsightPowerShellHardCodes.AzureHDInsightStorage)]
    public class AddAzureHDInsightStorageCmdlet : AzureHDInsightCmdlet, IAddAzureHDInsightStorageBase
    {
        private IAddAzureHDInsightStorageCommand command;

        /// <summary>
        /// Initializes a new instance of the AddAzureHDInsightStorageCmdlet class.
        /// </summary>
        public AddAzureHDInsightStorageCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateAddStorage();
        }

        /// <summary>
        /// Gets or sets the Azure HDInsight Configuration for the Azure HDInsight cluster being constructed.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The HDInsight cluster configuration to use when creating the new cluster (created by New-AzureHDInsightConfig).",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddStorageAccount)]
        public AzureHDInsightConfig Config
        {
            get { return this.command.Config; }
            set
            {
                if (value.IsNull())
                {
                    throw new ArgumentNullException("value",
                                                    "The value for the configuration can not be null.");
                }
                this.command.Config.ClusterSizeInNodes = value.ClusterSizeInNodes;
                this.command.Config.DefaultStorageAccount = value.DefaultStorageAccount;
                this.command.Config.HiveMetastore = value.HiveMetastore ?? this.command.Config.HiveMetastore;
                this.command.Config.OozieMetastore = value.OozieMetastore ?? this.command.Config.OozieMetastore;
                this.command.Config.AdditionalStorageAccounts.AddRange(value.AdditionalStorageAccounts);
            }
        }

        /// <summary>
        /// Gets or sets the Storage Account Name for the storage account to be added to the cluster.
        /// </summary>
        [Parameter(Position = 1, Mandatory = true,
            HelpMessage = "The storage account name for the storage account to be added to the new cluster.",
            ValueFromPipeline = false,
            ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddStorageAccount)]
        public string StorageAccountName
        {
            get { return this.command.StorageAccountName; }
            set { this.command.StorageAccountName = value; }
        }

        /// <summary>
        /// Gets or sets the Storage Account key for the storage account to be added to the cluster.
        /// </summary>
        [Parameter(Position = 2, Mandatory = true,
            HelpMessage = "The storage account key for the storage account to be added to the new cluster.",
            ValueFromPipeline = false,
            ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddStorageAccount)]
        public string StorageAccountKey
        {
            get { return this.command.StorageAccountKey; }
            set { this.command.StorageAccountKey = value; }
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            this.command.EndProcessing();
            foreach (var output in this.command.Output)
            {
                this.WriteObject(output);
            }
        }
    }
}
