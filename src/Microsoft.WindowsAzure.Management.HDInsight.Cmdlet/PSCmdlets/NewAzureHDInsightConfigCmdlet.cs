namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Represents the New-AzureHDInsightConfig Power Shell Cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.New, AzureHdInsightPowerShellHardCodes.AzureHDInsightConfig)]
    public class NewAzureHDInsightConfigCmdlet : AzureHDInsightCmdlet, INewAzureHDInsightConfigBase
    {
        private INewAzureHDInsightConfigCommand configCommand;

        /// <summary>
        /// Initializes a new instance of the NewAzureHDInsightConfigCmdlet class.
        /// </summary>
        public NewAzureHDInsightConfigCmdlet()
        {
            this.configCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewConfig();
        }

        /// <summary>
        /// Finishes the execution of the cmdlet by listing the clusters.
        /// </summary>
        protected override void EndProcessing()
        {
            this.configCommand.EndProcessing();
            foreach (var output in this.configCommand.Output)
            {
                this.WriteObject(output);
            }
        }

        /// <inheritdoc />
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The number of data nodes to use for the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetConfigClusterSizeInNodesOnly)]
        [Alias(AzureHdInsightPowerShellHardCodes.AliasNodes, AzureHdInsightPowerShellHardCodes.AliasSize)]
        public int ClusterSizeInNodes
        {
            get { return this.configCommand.ClusterSizeInNodes; }
            set { this.configCommand.ClusterSizeInNodes = value; }
        }

    }
}
