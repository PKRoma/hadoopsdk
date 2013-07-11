namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;

    internal abstract class AzureHDInsightClusterCommandBase : AzureHDInsightCommandBase, IAzureHDInsightClusterCommandBase
    {
        public Guid SubscriptionId { get; set; }

        public X509Certificate2 Certificate { get; set; }
        
        public Uri EndPoint { get; set; }
        
        public string CloudServiceName { get; set; }

        protected IClusterProvisioningClient GetClient()
        {
            IClusterProvisioningClient client;
            if (this.CloudServiceName.IsNotNullOrEmpty() && this.EndPoint.IsNotNull())
            {
                client = ServiceLocator.Instance.Locate<IClusterProvisioningClientFactory>().Create(this.SubscriptionId,
                                                                                                    this.Certificate,
                                                                                                    this.EndPoint,
                                                                                                    this.CloudServiceName);
            }
            else if (this.EndPoint.IsNotNull())
            {
                client = ServiceLocator.Instance.Locate<IClusterProvisioningClientFactory>().Create(this.SubscriptionId,
                                                                                                    this.Certificate,
                                                                                                    this.EndPoint);
            }
            else
            {
                client = ServiceLocator.Instance.Locate<IClusterProvisioningClientFactory>().Create(this.SubscriptionId,
                                                                                                    this.Certificate);
            }
            return client;
        }

        public string Name { get; set; }
    }
}
