namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class RemoveAzureHDInsightClusterCommand : AzureHDInsightClusterCommandBase, IRemoveAzureHDInsightClusterCommand
    {
        /// <inheritdoc />
        public string Location { get; set; }

        public override void EndProcessing()
        {
            var client = this.GetClient();

            if (!string.IsNullOrWhiteSpace(this.Location))
            {
                client.DeleteCluster(this.Name);
            }
            else
            {
                client.DeleteCluster(this.Name);
            }
            
        }
    }
}
