namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Cmdlet that deletes a cluster from the HDInsight service.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, AzureHdInsightPowerShellHardCodes.AzureHDInsightCluster)]
    public class RemoveClusterHDInsightClusterCmdlet : AzureHDInsightCmdlet, IRemoveAzureHDInsightClusterBase
    {
        private IRemoveAzureHDInsightClusterCommand deleteCommand;

        /// <summary>
        /// Initializes a new instance of the RemoveClusterHDInsightClusterCmdlet class.
        /// </summary>
        public RemoveClusterHDInsightClusterCmdlet()
        {
            this.deleteCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateDelete();
        }

        /// <inheritdoc />
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The name of the cluster to remove.",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasClusterName, AzureHdInsightPowerShellHardCodes.AliasDnsName)]
        public string Name
        {
            get { return this.deleteCommand.Name; }
            set { this.deleteCommand.Name = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true, 
                   HelpMessage = "The subscription id for the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasSub, AzureHdInsightPowerShellHardCodes.AliasSubscription)]
        public Guid SubscriptionId
        {
            get { return this.deleteCommand.SubscriptionId; }
            set { this.deleteCommand.SubscriptionId = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 2, Mandatory = true,
                   HelpMessage = "The management certificate used to manage the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.deleteCommand.Certificate; }
            set { this.deleteCommand.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 3, Mandatory = false,
                   HelpMessage = "The azure location where the new cluster should be created.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasLoc)]
        public string Location
        {
            get { return this.deleteCommand.Location; }
            set { this.deleteCommand.Location = value; }
        }

        /// <summary>
        /// Finishes the execution of the cmdlet by listing the clusters.
        /// </summary>
        protected override void EndProcessing()
        {
            this.deleteCommand.EndProcessing();
        }
    }
}