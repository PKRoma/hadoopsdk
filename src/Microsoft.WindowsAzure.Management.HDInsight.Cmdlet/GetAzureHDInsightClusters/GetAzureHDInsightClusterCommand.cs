namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework;

    internal class GetAzureHDInsightClusterCommand : AzureHDInsightClusterCommand<AzureHDInsightCluster>, IGetAzureHDInsightClusterCommand
    {
        public override void EndProcessing()
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightSyncClientFactory>().Create(this.SubscriptionId,
                                                                                              this.Certificate);

            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                this.Output.Add(new AzureHDInsightCluster(client.ListContainer(this.Name)));
            }
            else
            {
                this.Output.AddRange(client.ListContainers().Select(c => new AzureHDInsightCluster(c)));
            }
        }
    }
}