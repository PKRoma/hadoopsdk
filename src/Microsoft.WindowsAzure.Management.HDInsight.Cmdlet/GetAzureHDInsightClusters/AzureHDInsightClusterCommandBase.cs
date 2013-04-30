namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    internal abstract class AzureHDInsightClusterCommandBase : AzureHDInsightCommandBase, IAzureHDInsightClusterCommandBase
    {
        public Guid SubscriptionId { get; set; }

        public X509Certificate2 Certificate { get; set; }

        public string Name { get; set; }
    }
}
