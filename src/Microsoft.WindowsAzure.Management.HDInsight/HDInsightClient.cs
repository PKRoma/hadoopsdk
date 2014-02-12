// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.
namespace Microsoft.WindowsAzure.Management.HDInsight
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.WebHCatRest;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.ClusterManager;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.VersionFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
        Justification =
            "DisposableObject implements IDisposable correctly, the implementation of IDisposable in the interfaces is necessary for the design.")]
    public sealed class HDInsightClient : ClientBase, IHDInsightClient
    {
        /// <summary>
        ///     Default HDInsight version.
        /// </summary>
        internal const string DEFAULTHDINSIGHTVERSION = "default";

        internal const string ClusterAlreadyExistsError = "The condition specified by the ETag is not satisfied.";

        private IHDInsightSubscriptionCredentials credentials;

        /// <summary>
        /// Gets the connection credential.
        /// </summary>
        public IHDInsightSubscriptionCredentials Credentials
        {
            get { return this.credentials; }
        }

        /// <inheritdoc />
        public TimeSpan PollingInterval { get; set; }

        /// <inheritdoc />
        internal static TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Initializes a new instance of the HDInsightClient class.
        /// </summary>
        /// <param name="credentials">
        /// The credential to use when operating against the service.
        /// </param>
        internal HDInsightClient(IHDInsightSubscriptionCredentials credentials)
        {
            var asCertificateCredentials = credentials;
            if (asCertificateCredentials.IsNull())
            {
                throw new InvalidOperationException("Unable to connect to the HDInsight subscription with the supplied type of credential");
            }
            this.credentials = ServiceLocator.Instance.Locate<IHDInsightSubscriptionCredentialsFactory>().Create(asCertificateCredentials);
            this.PollingInterval = DefaultPollingInterval;
        }

        /// <summary>
        /// Connects to an HDInsight subscription.
        /// </summary>
        /// <param name="credentials">
        /// The credential used to connect to the subscription.
        /// </param>
        /// <returns>
        /// A new HDInsight client.
        /// </returns>
        public static IHDInsightClient Connect(IHDInsightSubscriptionCredentials credentials)
        {
            return ServiceLocator.Instance.Locate<IHDInsightClientFactory>().Create(credentials);
        }

        /// <inheritdoc />
        public event EventHandler<ClusterProvisioningStatusEventArgs> ClusterProvisioning;

        /// <inheritdoc />
        public async Task<Collection<string>> ListAvailableLocationsAsync()
        {
            var client = ServiceLocator.Instance.Locate<ILocationFinderClientFactory>().Create(this.credentials, this.Context);
            return await client.ListAvailableLocations();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<KeyValuePair<string, string>>> ListResourceProviderPropertiesAsync()
        {
            var client = ServiceLocator.Instance.Locate<IRdfeServiceRestClientFactory>().Create(this.credentials, this.Context);
            return await client.GetResourceProviderProperties();
        }

        /// <inheritdoc />
        public async Task<Collection<HDInsightVersion>> ListAvailableVersionsAsync()
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context);
            return await overrideHandlers.VersionFinder.ListAvailableVersions();
        }

        /// <inheritdoc />
        public async Task<ICollection<ClusterDetails>> ListClustersAsync()
        {
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context))
            {
                return await client.ListContainers();
            }
        }

        /// <inheritdoc />
        public async Task<ClusterDetails> GetClusterAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context))
            {
                return await client.ListContainer(name);
            }
        }

        /// <inheritdoc />
        public async Task<ClusterDetails> CreateClusterAsync(ClusterCreateParameters clusterCreateParameters)
        {
            if (clusterCreateParameters == null)
            {
                throw new ArgumentNullException("clusterCreateParameters");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context);
            await this.ValidateClusterVersion(clusterCreateParameters);

            // listen to cluster provisioning events on the POCO client.
            client.ClusterProvisioning += this.RaiseClusterProvisioningEvent;
            Exception requestException = null;

            // Creates a cluster and waits for it to complete
            try
            {
                await client.CreateContainer(clusterCreateParameters);
            }
            catch (AggregateException aex)
            {
                var ex = aex.GetInnerException();
                var layerException = ex as HttpLayerException;
                if (layerException != null)
                {
                    requestException = layerException;
                    HandleCreateHttpLayerException(clusterCreateParameters, layerException);
                }
                else
                {
                    requestException = ex as HttpRequestException;
                }
            }
            catch (HttpLayerException e)
            {
                requestException = e;
                HandleCreateHttpLayerException(clusterCreateParameters, e);
            }
            catch (HttpRequestException rex)
            {
                requestException = rex;
            }
            await client.WaitForClusterInConditionOrError(this.HandleClusterWaitNotifyEvent,
                                                          clusterCreateParameters.Name,
                                                          clusterCreateParameters.CreateTimeout,
                                                          this.PollingInterval,
                                                          this.Context,
                                                          ClusterState.Operational,
                                                          ClusterState.Running);

            // Validates that cluster didn't get on error state
            var result = await this.GetClusterAsync(clusterCreateParameters.Name);
            if (result == null)
            {
                if (requestException != null)
                {
                    throw requestException;
                }
                throw new OperationCanceledException("Attempting to return the newly created cluster returned no cluster.  The cluster could not be found.");
            }
            if (result.Error != null)
            {
                throw new OperationCanceledException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to complete the '{0}' operation. Operation failed with code '{1}'. Cluster left behind state: '{2}'. Message: '{3}'.",
                        result.Error.OperationType ?? "UNKNOWN",
                        result.Error.HttpCode,
                        result.Error.Message ?? "NULL",
                        result.StateString ?? "NULL"));
            }

            return result;
        }

        private static void HandleCreateHttpLayerException(ClusterCreateParameters clusterCreateParameters, HttpLayerException e)
        {
            if (e.RequestContent.Contains(ClusterAlreadyExistsError) && e.RequestStatusCode == HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cluster {0} already exists.", clusterCreateParameters.Name));
            }
        }

        /// <summary>
        /// Raises the cluster provisioning event.
        /// </summary>
        /// <param name="sender">The IHDInsightManagementPocoClient instance.</param>
        /// <param name="e">EventArgs for the event.</param>
        public void RaiseClusterProvisioningEvent(object sender, ClusterProvisioningStatusEventArgs e)
        {
            var handler = this.ClusterProvisioning;
            if (handler.IsNotNull())
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Used to handle the notification events during waiting.
        /// </summary>
        /// <param name="cluster">
        /// The cluster in its current state.
        /// </param>
        public void HandleClusterWaitNotifyEvent(ClusterDetails cluster)
        {
            if (cluster.IsNotNull())
            {
                this.RaiseClusterProvisioningEvent(this, new ClusterProvisioningStatusEventArgs(cluster, cluster.State));
            }
        }

        /// <inheritdoc />
        public async Task DeleteClusterAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context);
            await client.DeleteContainer(name);
            await client.WaitForClusterNull(name, TimeSpan.FromMinutes(30), this.Context.CancellationToken);
        }

        /// <inheritdoc />
        public async Task EnableHttpAsync(string dnsName, string location, string httpUserName, string httpPassword)
        {
            dnsName.ArgumentNotNullOrEmpty("dnsName");
            location.ArgumentNotNullOrEmpty("location");
            httpUserName.ArgumentNotNullOrEmpty("httpUserName");
            httpPassword.ArgumentNotNullOrEmpty("httpPassword");

            await this.AssertClusterVersionSupported(dnsName);

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context);
            var operationId = await client.EnableHttp(dnsName, location, httpUserName, httpPassword);
            await client.WaitForOperationCompleteOrError(dnsName, location, operationId, TimeSpan.FromHours(1), this.Context.CancellationToken);
        }

        /// <inheritdoc />
        public void DisableHttp(string dnsName, string location)
        {
            this.DisableHttpAsync(dnsName, location).WaitForResult();
        }

        /// <inheritdoc />
        public void EnableHttp(string dnsName, string location, string httpUserName, string httpPassword)
        {
            this.EnableHttpAsync(dnsName, location, httpUserName, httpPassword).WaitForResult();
        }

        /// <inheritdoc />
        public async Task DisableHttpAsync(string dnsName, string location)
        {
            dnsName.ArgumentNotNullOrEmpty("dnsName");
            location.ArgumentNotNullOrEmpty("location");

            await this.AssertClusterVersionSupported(dnsName);

            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(this.credentials, this.Context);
            var operationId = await client.DisableHttp(dnsName, location);
            await client.WaitForOperationCompleteOrError(dnsName, location, operationId, TimeSpan.FromHours(1), this.Context.CancellationToken);
        }

        /// <inheritdoc />
        public Collection<string> ListAvailableLocations()
        {
            return this.ListAvailableLocationsAsync().WaitForResult();
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, string>> ListResourceProviderProperties()
        {
            var client = ServiceLocator.Instance.Locate<IRdfeServiceRestClientFactory>().Create(this.credentials, this.Context);
            return client.GetResourceProviderProperties().WaitForResult();
        }

        /// <inheritdoc />
        public Collection<HDInsightVersion> ListAvailableVersions()
        {
            return this.ListAvailableVersionsAsync().WaitForResult();
        }

        /// <inheritdoc />
        public ICollection<ClusterDetails> ListClusters()
        {
            return this.ListClustersAsync().WaitForResult();
        }

        /// <inheritdoc />
        public ClusterDetails GetCluster(string dnsName)
        {
            return this.GetClusterAsync(dnsName).WaitForResult();
        }

        /// <inheritdoc />
        public ClusterDetails CreateCluster(ClusterCreateParameters cluster)
        {
            return this.CreateClusterAsync(cluster).WaitForResult();
        }

        /// <inheritdoc />
        public ClusterDetails CreateCluster(ClusterCreateParameters cluster, TimeSpan timeout)
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

        // This method is used by the NonPublic SDK.  Be aware of braking changes to that project when you alter it.
        private async Task AssertClusterVersionSupported(string dnsName)
        {
            var cluster = await this.GetClusterAsync(dnsName);
            if (cluster == null)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cluster '{0}' was not found in your subscription {1}.",
                        dnsName,
                        this.credentials.SubscriptionId));
            }

            this.AssertSupportedVersion(cluster.VersionNumber);
        }

        private async Task ValidateClusterVersion(ClusterCreateParameters cluster)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context);
            // Validates the version for cluster creation
            if (string.IsNullOrEmpty(cluster.Version) || string.Equals(cluster.Version, DEFAULTHDINSIGHTVERSION, StringComparison.OrdinalIgnoreCase))
            {
                cluster.Version = DEFAULTHDINSIGHTVERSION;
            }
            else
            {
                this.AssertSupportedVersion(overrideHandlers.PayloadConverter.ConvertStringToVersion(cluster.Version));
                var availableVersions = await overrideHandlers.VersionFinder.ListAvailableVersions();
                if (availableVersions.All(hdinsightVersion => hdinsightVersion.Version != cluster.Version))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Cannot create a cluster with version '{0}'. Available Versions for your subscription are: {1}",
                            cluster.Version,
                            string.Join(",", availableVersions)));
                }
            }
        }

        private void AssertSupportedVersion(Version hdinsightClusterVersion)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context);
            switch (overrideHandlers.VersionFinder.GetVersionStatus(hdinsightClusterVersion))
            {
                case VersionStatus.Obsolete:
                    throw new NotSupportedException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            HDInsightConstants.ClusterVersionTooLowForClusterOperations,
                            hdinsightClusterVersion.ToString(),
                            HDInsightSDKSupportedVersions.MinVersion,
                            HDInsightSDKSupportedVersions.MaxVersion));

                case VersionStatus.ToolsUpgradeRequired:
                    throw new NotSupportedException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            HDInsightConstants.ClusterVersionTooHighForClusterOperations,
                            hdinsightClusterVersion.ToString(),
                            HDInsightSDKSupportedVersions.MinVersion,
                            HDInsightSDKSupportedVersions.MaxVersion));
            }
        }
    }
}
