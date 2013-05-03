namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <inheritdoc />
    public class HDInsightClient : IHDInsightClient
    {
        /// <summary>
        /// Internal property that stores the connection logic.
        /// </summary>
        private readonly IConnectionCredentials credentials;

        /// <inheritdoc />
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Initializes a new instance of the HDInsightClient class.
        /// </summary>
        /// <param name="subscriptionId">Subscription to connect to.</param>
        /// <param name="certificate">Client certificate that has been enabled in the subscription.</param>
        public HDInsightClient(Guid subscriptionId, X509Certificate2 certificate)
        {
            this.PollingInterval = TimeSpan.FromSeconds(5);
            this.credentials = ServiceLocator.Instance.Locate<IConnectionCredentialsFactory>().Create(subscriptionId, certificate);
        }

        /// <inheritdoc />
        public async Task<Collection<ListClusterContainerResult>> ListContainers()
        {
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials))
            {
                return await client.ListContainers();
            }
        }

        /// <inheritdoc />
        public async Task<ListClusterContainerResult> ListContainer(string dnsName)
        {
            if (dnsName == null)
            {
                throw new ArgumentNullException("dnsName");
            }
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials))
            {
                return await client.ListContainer(dnsName);
            }
        }

        /// <inheritdoc />
        public async Task<ListClusterContainerResult> CreateContainer(CreateClusterRequest cluster)
        {
            if (cluster == null)
            {
                throw new ArgumentNullException("cluster");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials);

            // Creates a cluster and waits for it to complete
            await client.CreateContainer(cluster);
            client.WaitForClusterCondition(
                cluster.DnsName,
                c =>
                c != null && (c.ResultError != null ||
                              c.ParsedState == ClusterState.Operational ||
                              c.ParsedState == ClusterState.Running),
                this.PollingInterval);

            // Validates that cluster didn't get on error state
            var result = await this.ListContainer(cluster.DnsName);
            if (result.ResultError != null)
            {
                throw new OperationCanceledException(string.Format(CultureInfo.InvariantCulture,
                                                                "Unable to complete the '{0}' operation. Operation failed with code '{1}'. Cluster left behind state: '{2}'. Message: '{3}'.",
                                                                result.ResultError.OperationType ?? "UNKNOWN",
                                                                result.ResultError.HttpCode,
                                                                result.ResultError.Message ?? "NULL",
                                                                result.State ?? "NULL"));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task DeleteContainer(string dnsName)
        {
            if (dnsName == null)
            {
                throw new ArgumentNullException("dnsName");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials);
            await client.DeleteContainer(dnsName);
            client.WaitForClusterCondition(dnsName, result => result == null, this.PollingInterval);
        }
    }
}