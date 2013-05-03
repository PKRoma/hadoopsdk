namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <inheritdoc />
    public class HDInsightSyncClient : IHDInsightSyncClient
    {
        /// <summary>
        /// Internal property that stores the inner client.
        /// </summary>
        private readonly IHDInsightClient asyncClient;

        /// <inheritdoc />
        public TimeSpan PollingInterval
        {
            get { return this.asyncClient.PollingInterval; } 
            set { this.asyncClient.PollingInterval = value; }
        }

        /// <summary>
        /// Initializes a new instance of the HDInsightSyncClient class.
        /// </summary>
        /// <param name="subscriptionId">Subscription to connect to.</param>
        /// <param name="certificate">Client certificate that has been enabled in the subscription.</param>
        public HDInsightSyncClient(Guid subscriptionId, X509Certificate2 certificate)
        {
            this.asyncClient = ServiceLocator.Instance.Locate<IHDInsightClientFactory>().Create(subscriptionId, certificate);
        }
        
        /// <inheritdoc />
        public Collection<ListClusterContainerResult> ListContainers()
        {
            return this.asyncClient.ListContainers().WaitForResult();
        }

        /// <inheritdoc />
        public ListClusterContainerResult ListContainer(string dnsName)
        {
            return this.asyncClient.ListContainer(dnsName).WaitForResult();
        }

        /// <inheritdoc />
        public ListClusterContainerResult CreateContainer(CreateClusterRequest cluster)
        {
            return this.asyncClient.CreateContainer(cluster).WaitForResult();
        }

        /// <inheritdoc />
        public ListClusterContainerResult CreateContainer(CreateClusterRequest cluster, TimeSpan timeout)
        {
            return this.asyncClient.CreateContainer(cluster).WaitForResult(timeout);
        }

        /// <inheritdoc />
        public void DeleteContainer(string dnsName)
        {
            this.asyncClient.DeleteContainer(dnsName).WaitForResult();
        }

        /// <inheritdoc />
        public void DeleteContainer(string dnsName, TimeSpan timeout)
        {
            this.asyncClient.DeleteContainer(dnsName).WaitForResult(timeout);
        }
    }
}
