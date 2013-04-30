namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class NewAzureHDInsightConfigCommand : AzureHDInsightCommand<AzureHDInsightConfig>, INewAzureHDInsightConfigCommand
    {
        private AzureHDInsightConfig config = new AzureHDInsightConfig();

        /// <summary>
        /// Gets or sets the size of the cluster in worker nodes.
        /// </summary>
        public int ClusterSizeInNodes
        {
            get { return this.config.ClusterSizeInNodes; }
            set { this.config.ClusterSizeInNodes = value; }
        }

        public override void EndProcessing()
        {
            this.Output.Add(this.config);
        }
    }
}
