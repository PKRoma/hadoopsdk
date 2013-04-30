namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Cmdlet that creates a new HDInsight cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.New, AzureHdInsightPowerShellHardCodes.AzureHDInsightCluster)]
    public class NewAzureHDInsightClusterCmdlet : AzureHDInsightCmdlet
    {
        private INewAzureHDInsightClusterCommand createCommand;

        /// <summary>
        /// Initializes a new instance of the NewAzureHDInsightClusterCmdlet class.
        /// </summary>
        public NewAzureHDInsightClusterCmdlet()
        {
            this.createCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateCreate();
        }

        /// <inheritdoc />
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            this.createCommand.EndProcessing();
            foreach (var output in this.createCommand.Output)
            {
                this.WriteObject(output);
            }
        }

        /// <inheritdoc />
        [Parameter(Position = 0, Mandatory = true, 
                   HelpMessage = "The name of the HDInsight cluster to locate.",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The name of the HDInsight cluster to locate.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasClusterName, AzureHdInsightPowerShellHardCodes.AliasDnsName)]
        public string Name
        {
            get { return this.createCommand.Name; }
            set { this.createCommand.Name = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true,
                   HelpMessage = "The HDInsight cluster configuration to use when creating the new cluster (created by New-AzureHDInsightConfig).",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        public AzureHDInsightConfig Config
        {
            get
            {
                var result = new AzureHDInsightConfig();
                result.ClusterSizeInNodes = this.createCommand.ClusterSizeInNodes;
                result.DefaultStorageAccount.StorageAccountName = this.createCommand.DefaultStorageAccountName;
                result.DefaultStorageAccount.StorageAccountKey = this.createCommand.DefaultStorageAccountKey;
                result.DefaultStorageAccount.StorageContainerName = this.createCommand.DefaultStorageContainerName;
                result.AdditionalStorageAccounts.AddRange(this.createCommand.AdditionalStorageAccounts);
                return result;
            }
            
            set
            {
                if (value.IsNull())
                {
                    throw new ArgumentNullException("value", "The value for the configuration can not be null.");
                }
                this.createCommand.ClusterSizeInNodes = value.ClusterSizeInNodes;
                this.createCommand.DefaultStorageAccountName = value.DefaultStorageAccount.StorageAccountName;
                this.createCommand.DefaultStorageAccountKey = value.DefaultStorageAccount.StorageAccountKey;
                this.createCommand.DefaultStorageContainerName = value.DefaultStorageAccount.StorageContainerName;
                this.createCommand.AdditionalStorageAccounts.AddRange(value.AdditionalStorageAccounts);
                this.createCommand.HiveMetastore = value.HiveMetastore;
                this.createCommand.OozieMetastore = value.OozieMetastore;
            }
        }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true,
                   HelpMessage = "The subscription id for the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 2, Mandatory = true,
                   HelpMessage = "The subscription id for the Azure subscription.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasSub, AzureHdInsightPowerShellHardCodes.AliasSubscription)]
        public Guid SubscriptionId
        {
            get { return this.createCommand.SubscriptionId; }
            set { this.createCommand.SubscriptionId = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 2, Mandatory = true,
                   HelpMessage = "The management certificate used to manage the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 3, Mandatory = true,
                   HelpMessage = "The management certificate used to manage the Azure subscription.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.createCommand.Certificate; }
            set { this.createCommand.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 3, Mandatory = true,
                   HelpMessage = "The azure location where the new cluster should be created.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 4, Mandatory = true,
                   HelpMessage = "The azure location where the new cluster should be created.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasLoc)]
        public string Location
        {
            get { return this.createCommand.Location; }
            set { this.createCommand.Location = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 4, Mandatory = true,
                   HelpMessage = "The default storage account to use for the new cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasStorageAccount)]
        public string DefaultStorageAccountName
        {
            get { return this.createCommand.DefaultStorageAccountName; }
            set { this.createCommand.DefaultStorageAccountName = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 5, Mandatory = true,
                   HelpMessage = "The key to use for the default storage account.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasStorageKey)]
        public string DefaultStorageAccountKey
        {
            get { return this.createCommand.DefaultStorageAccountKey; }
            set { this.createCommand.DefaultStorageAccountKey = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 6, Mandatory = true,
                   HelpMessage = "The container in the storage account to use for default HDInsight storage.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasStorageContainer)]
        public string DefaultStorageContainerName
        {
            get { return this.createCommand.DefaultStorageContainerName; }
            set { this.createCommand.DefaultStorageContainerName = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 7, Mandatory = true,
                   HelpMessage = "The user name to use for the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 5, Mandatory = true,
                   HelpMessage = "The user name to use for the HDInsight cluster.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasUser)]
        public string UserName
        {
            get { return this.createCommand.UserName; }
            set { this.createCommand.UserName = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 8, Mandatory = true,
                   HelpMessage = "The password to use for the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Parameter(Position = 6, Mandatory = true,
                   HelpMessage = "The password to use for the HDInsight cluster.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByConfigWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasPwd)]
        public string Password
        {
            get { return this.createCommand.Password; }
            set { this.createCommand.Password = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 9, Mandatory = true,
                   HelpMessage = "The number of data nodes to use for the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasNodes, AzureHdInsightPowerShellHardCodes.AliasSize)]
        public int ClusterSizeInNodes
        {
            get { return this.createCommand.ClusterSizeInNodes; }
            set { this.createCommand.ClusterSizeInNodes = value; }
        }
    }
}
