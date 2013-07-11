namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Cmdlet that lists all the clusters registered in the HDInsight service.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, AzureHdInsightPowerShellHardCodes.AzureHDInsightCluster)]
    public class GetAzureHDInsightClusterCmdlet : AzureHDInsightCmdlet, IGetAzureHDInsightClusterBase
    {
        private IGetAzureHDInsightClusterCommand getCommand;

        /// <summary>
        /// Initializes a new instance of the GetAzureHDInsightClusterCmdlet class.
        /// </summary>
        public GetAzureHDInsightClusterCmdlet()
        {
            this.getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
        }

        /// <inheritdoc />
        [Parameter(Position = 0, Mandatory = false,
                   HelpMessage = "The name of the HDInsight cluster to locate.",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasClusterName, AzureHdInsightPowerShellHardCodes.AliasDnsName)]
        public string Name
        {
            get { return this.getCommand.Name; }
            set { this.getCommand.Name = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true, 
                   HelpMessage = "The subscription id for the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasSub, AzureHdInsightPowerShellHardCodes.AliasSubscription)]
        public Guid SubscriptionId
        {
            get { return this.getCommand.SubscriptionId; }
            set { this.getCommand.SubscriptionId = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 2, Mandatory = true, 
                   HelpMessage = "The management certificate used to manage the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.getCommand.Certificate; }
            set { this.getCommand.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 3, Mandatory = false,
                   HelpMessage = "The Endpoint to use when connecting to Azure.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        public Uri EndPoint
        {
            get { return this.getCommand.EndPoint; }
            set { this.getCommand.EndPoint = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 4, Mandatory = false,
                   HelpMessage = "The CloudServiceName to use when managing the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetClusterByNameWithSpecificSubscriptionCredentails)]
        public string CloudServiceName
        {
            get { return this.getCommand.CloudServiceName; }
            set { this.getCommand.CloudServiceName = value; }
        }

        /// <summary>
        /// Finishes the execution of the cmdlet by listing the clusters.
        /// </summary>
        protected override void EndProcessing()
        {
            this.getCommand.EndProcessing();
            foreach (var output in this.getCommand.Output)
            {
                this.WriteObject(output);
            }
        }
    }
}