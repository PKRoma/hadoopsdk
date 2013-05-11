namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    /// <inheritdoc />
    public class ClusterProvisioningClient : IClusterProvisioningClient
    {
        /// <summary>
        /// Internal property that stores the connection logic.
        /// </summary>
        private readonly IConnectionCredentials credentials;

        /// <inheritdoc />
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Initializes a new instance of the ClusterProvisioningClient class.
        /// </summary>
        /// <param name="subscriptionId">Subscription to connect to.</param>
        /// <param name="certificate">Client certificate that has been enabled in the subscription.</param>
        public ClusterProvisioningClient(Guid subscriptionId, X509Certificate2 certificate)
        {
            this.PollingInterval = TimeSpan.FromSeconds(5);
            this.credentials = ServiceLocator.Instance.Locate<IConnectionCredentialsFactory>().Create(subscriptionId, certificate);
        }

        /// <inheritdoc />
        public async Task<Collection<HDInsightCluster>> ListClustersAsync()
        {
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials))
            {
                return await client.ListContainers();
            }
        }

        /// <inheritdoc />
        public async Task<HDInsightCluster> GetClusterAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials))
            {
                return await client.ListContainer(name);
            }
        }

        /// <inheritdoc />
        public async Task<HDInsightCluster> CreateClusterAsync(HDInsightClusterCreationDetails cluster)
        {
            if (cluster == null)
            {
                throw new ArgumentNullException("cluster");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials);

            // Creates a cluster and waits for it to complete
            await client.CreateContainer(cluster);
            client.WaitForClusterCondition(
                cluster.Name,
                c =>
                c != null && (c.Error != null ||
                              c.State == ClusterState.Operational ||
                              c.State == ClusterState.Running),
                this.PollingInterval);

            // Validates that cluster didn't get on error state
            var result = await this.GetClusterAsync(cluster.Name);
            if (result.Error != null)
            {
                throw new OperationCanceledException(string.Format(CultureInfo.InvariantCulture,
                                                                "Unable to complete the '{0}' operation. Operation failed with code '{1}'. Cluster left behind state: '{2}'. Message: '{3}'.",
                                                                result.Error.OperationType ?? "UNKNOWN",
                                                                result.Error.HttpCode,
                                                                result.Error.Message ?? "NULL",
                                                                result.StateString ?? "NULL"));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task DeleteClusterAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials);
            await client.DeleteContainer(name);
            client.WaitForClusterCondition(name, result => result == null, this.PollingInterval);
        }

        /// <inheritdoc />
        public Collection<HDInsightCluster> ListClusters()
        {
            return this.ListClustersAsync().WaitForResult();
        }

        /// <inheritdoc />
        public HDInsightCluster GetCluster(string dnsName)
        {
            return this.GetClusterAsync(dnsName).WaitForResult();
        }

        /// <inheritdoc />
        public HDInsightCluster CreateCluster(HDInsightClusterCreationDetails cluster)
        {
            return this.CreateClusterAsync(cluster).WaitForResult();
        }

        /// <inheritdoc />
        public HDInsightCluster CreateCluster(HDInsightClusterCreationDetails cluster, TimeSpan timeout)
        {
            return this.CreateClusterAsync(cluster).WaitForResult(timeout);
        }

        /// <inheritdoc />
        public void DeleteCluster(string dnsName)
        {
            this.DeleteClusterAsync(dnsName).WaitForResult();
        }

        /// <inheritdoc />
        public void DeleteCluster(string dnsName, TimeSpan timeout)
        {
            this.DeleteClusterAsync(dnsName).WaitForResult(timeout);
        }
    }
}