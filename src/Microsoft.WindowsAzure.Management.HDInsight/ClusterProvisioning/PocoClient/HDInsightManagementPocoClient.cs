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
namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.ClusterManager;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal class HDInsightManagementPocoClient : DisposableObject, IHDInsightManagementPocoClient
    {
        internal const string ClusterCrudCapabilitityName = "CAPABILITY_FEATURE_CUSTOM_ACTIONS_V2";
        internal const string HighAvailabilityCapabilitityName = "CAPABILITY_FEATURE_HIGH_AVAILABILITY";

        private readonly IHDInsightSubscriptionCredentials credentials;
        private readonly bool ignoreSslErrors;

        public IAbstractionContext Context { get; private set; }

        internal HDInsightManagementPocoClient(IHDInsightSubscriptionCredentials credentials, IAbstractionContext context, bool ignoreSslErrors)
        {
            this.Context = context;
            this.credentials = credentials;
            this.ignoreSslErrors = ignoreSslErrors;
            if (context.IsNotNull() && context.Logger.IsNotNull())
            {
                this.Logger = context.Logger;
            }
            else
            {
                this.Logger = new Logger();
            }
        }

        /// <inheritdoc />
        public event EventHandler<ClusterProvisioningStatusEventArgs> ClusterProvisioning;

        /// <inheritdoc />
        public void RaiseClusterProvisioningEvent(object sender, ClusterProvisioningStatusEventArgs e)
        {
            var handler = this.ClusterProvisioning;
            if (handler.IsNotNull())
            {
                handler(sender, e);
            }
        }

        public async Task<ICollection<ClusterDetails>> ListContainers()
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context, this.ignoreSslErrors);
            var response = await client.ListCloudServices();
            return overrideHandlers.PayloadConverter.DeserializeListContainersResult(response.Content, this.credentials.DeploymentNamespace, this.credentials.SubscriptionId);
        }

        public async Task<ClusterDetails> ListContainer(string dnsName)
        {
            var clusters = await this.ListContainers();
            var result = clusters.FirstOrDefault(cluster => cluster.Name.Equals(dnsName, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public void ValidateAsvAccounts(ClusterCreateParameters details)
        {
            var defaultStorageAccount = new WabStorageAccountConfiguration(
                details.DefaultStorageAccountName, details.DefaultStorageAccountKey, details.DefaultStorageContainer);
            // Flattens all the configurations into a single list for more uniform validation
            var asvList = ResolveStorageAccounts(details.AdditionalStorageAccounts).ToList();
            asvList.Add(ResolveStorageAccount(defaultStorageAccount));

            // Basic validation on the ASV configurations
            if (string.IsNullOrEmpty(details.DefaultStorageContainer))
            {
                throw new InvalidOperationException("Invalid Container. Default Storage Account Container cannot be null or empty");
            }
            if (asvList.Any(asv => string.IsNullOrEmpty(asv.Name) || string.IsNullOrEmpty(asv.Key)))
            {
                throw new InvalidOperationException("Invalid Azure Configuration. Credentials cannot be null or empty");
            }

            if (asvList.GroupBy(asv => asv.Name).Count(group => group.Count() > 1) > 0)
            {
                throw new InvalidOperationException("Invalid Azure Storage credential. Duplicated values detected");
            }

            // Validates that we can establish the connection to the ASV Names and the default container
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            asvList.ForEach(asv => client.ValidateAccount(asv.Name, asv.Key).WaitForResult());

            var resolvedAccounts = ResolveStorageAccounts(details.AdditionalStorageAccounts);
            details.AdditionalStorageAccounts.Clear();
            details.AdditionalStorageAccounts.AddRange(resolvedAccounts);

            var resolvedDefaultStorageAccount = ResolveStorageAccount(defaultStorageAccount);
            details.DefaultStorageAccountName = resolvedDefaultStorageAccount.Name;

            client.CreateContainerIfNotExists(details.DefaultStorageAccountName,
                                              details.DefaultStorageAccountKey,
                                              details.DefaultStorageContainer).WaitForResult();
        }

        public async Task CreateContainer(ClusterCreateParameters details)
        {
            this.LogMessage("Create Cluster Requested", Severity.Informational, Verbosity.Diagnostic);
            // Validates that the AzureStorage Configurations are valid.
            this.ValidateAsvAccounts(details);
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context, this.ignoreSslErrors);

            var rdfeCapabilitiesClient =
                ServiceLocator.Instance.Locate<IRdfeServiceRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            var capabilities = await rdfeCapabilitiesClient.GetResourceProviderProperties();
            if (!this.HasClusterCreateCapability(capabilities))
            {
                throw new InvalidOperationException(string.Format(
                    "Your subscription cannot create clusters, please contact Support"));
            }

            // Validates the region for the cluster creation
            var locationClient = ServiceLocator.Instance.Locate<ILocationFinderClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            var availableLocations = locationClient.ListAvailableLocations(capabilities);
            if (!availableLocations.Contains(details.Location, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(string.Format(
                        "Cannot create a cluster in '{0}'. Available Locations for your subscription are: {1}",
                        details.Location,
                        string.Join(",", availableLocations)));
            }

            AssertHighAvailibityCapabilityEnabled(capabilities, details);

            // Validates whether the subscription\location needs to be initialized
            var registrationClient = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            if (!await registrationClient.ValidateSubscriptionLocation(details.Location))
            {
                await registrationClient.RegisterSubscription();
                await registrationClient.RegisterSubscriptionLocation(details.Location);
            }

            // Creates the cluster
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            string payload = overrideHandlers.PayloadConverter.SerializeClusterCreateRequest(details);
            await client.CreateContainer(details.Name, details.Location, payload);
        }

        public async Task DeleteContainer(string dnsName)
        {
            var task = this.ListContainer(dnsName);
            await task;
            var cluster = task.Result;

            if (cluster == null)
            {
                throw new InvalidOperationException(string.Format("The cluster '{0}' doesn't exist.", dnsName));
            }
            await this.DeleteContainer(cluster.Name, cluster.Location);
        }

        public async Task DeleteContainer(string dnsName, string location)
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            await client.DeleteContainer(dnsName, location);
        }

        public async Task<Guid> EnableDisableProtocol(UserChangeRequestUserType requestType, UserChangeRequestOperationType operation, string dnsName, string location, string userName, string password, DateTimeOffset expiration)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context, this.ignoreSslErrors);
            var manager = ServiceLocator.Instance.Locate<IUserChangeRequestManager>();
            var handler = manager.LocateUserChangeRequestHandler(this.credentials.GetType(), requestType);
            var payload = handler.Item2(operation, userName, password, expiration);
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            var response = await client.EnableDisableUserChangeRequest(dnsName, location, requestType, payload);
            var resultId = overrideHandlers.PayloadConverter.DeserializeConnectivityResponse(response.Content);
            var pocoHelper = new HDInsightManagementPocoHelper();
            pocoHelper.ValidateResponse(resultId);
            return resultId.Data;
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "Need to do a non-throwing parse of the boolean value.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EnsureHighAvailability", Justification = "Needed to show proper error message.")]
        internal static void AssertHighAvailibityCapabilityEnabled(IEnumerable<KeyValuePair<string, string>> capabilities, ClusterCreateParameters details)
        {
            if (details.EnsureHighAvailability)
            {
                var headNodeHACapability = capabilities.FirstOrDefault(capability => capability.Key == HighAvailabilityCapabilitityName);
                if (headNodeHACapability.Key.IsNull())
                {
                    throw new InvalidOperationException("Your subscription cannot create clusters with EnsureHighAvailability set to true, please contact Support.");
                }
            }
        }

        // This method is used by the NonPublic SDK.  Be aware of braking changes to that project when you alter it.
        internal static async Task EnableDisableUser(IHDInsightSubscriptionAbstractionContext context,
                                                     UserChangeRequestUserType requestType,
                                                     UserChangeRequestOperationType operation,
                                                     string dnsName,
                                                     string location,
                                                     string userName,
                                                     string password,
                                                     DateTimeOffset expiration)
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(context.Credentials, context, false);
            Guid operationId = await EnableDisableUserPocoCall(context, requestType, operation, dnsName, location, userName, password, expiration);
            await client.WaitForOperationCompleteOrError(dnsName, location, operationId, TimeSpan.FromHours(1), context.CancellationToken);
        }

        // This method is used by the NonPublic SDK.  Be aware of braking changes to that project when you alter it.
        private static async Task<Guid> EnableDisableUserPocoCall(IHDInsightSubscriptionAbstractionContext context,
                                                                  UserChangeRequestUserType requestType,
                                                                  UserChangeRequestOperationType operation,
                                                                  string dnsName,
                                                                  string location,
                                                                  string userName,
                                                                  string password,
                                                                  DateTimeOffset expiration)
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(context.Credentials, context, false);
            var operationId = await client.EnableDisableProtocol(requestType, operation, dnsName, location, userName, password, expiration);
            return operationId;
        }

        // This method is used by the NonPublic SDK.  Be aware of braking changes to that project when you alter it.
        internal static void RegisterUserChangeRequestHandler(Type credentialsType,
                                                              UserChangeRequestUserType changeType,
                                                              Func<IHDInsightSubscriptionAbstractionContext, string, string, Uri> uriBuilder,
                                                              Func<UserChangeRequestOperationType, string, string, DateTimeOffset, string> payloadConverter)
        {
            var manager = ServiceLocator.Instance.Locate<IUserChangeRequestManager>();
            manager.RegisterUserChangeRequestHandler(credentialsType, changeType, uriBuilder, payloadConverter);
        }

        public async Task<Guid> EnableHttp(string dnsName, string location, string httpUserName, string httpPassword)
        {
            return await this.EnableDisableProtocol(UserChangeRequestUserType.Http,
                                                    UserChangeRequestOperationType.Enable,
                                                    dnsName,
                                                    location,
                                                    httpUserName,
                                                    httpPassword,
                                                    DateTimeOffset.MinValue);
        }

        public async Task<Guid> DisableHttp(string dnsName, string location)
        {
            return await this.EnableDisableProtocol(UserChangeRequestUserType.Http,
                                                    UserChangeRequestOperationType.Disable,
                                                    dnsName,
                                                    location,
                                                    string.Empty,
                                                    string.Empty,
                                                    DateTimeOffset.MinValue);
        }

        public async Task<bool> IsComplete(string dnsName, string location, Guid operationId)
        {
            var status = await this.GetStatus(dnsName, location, operationId);
            return status.State != UserChangeRequestOperationStatus.Pending;
        }

        public async Task<UserChangeRequestStatus> GetStatus(string dnsName, string location, Guid operationId)
        {
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials, this.Context, this.ignoreSslErrors);
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.Context, this.ignoreSslErrors);
            var response = await client.GetOperationStatus(dnsName, location, operationId);
            var responseObject = overrideHandlers.PayloadConverter.DeserializeConnectivityStatus(response.Content);
            return responseObject.Data;
        }

        private bool HasClusterCreateCapability(IEnumerable<KeyValuePair<string, string>> capabilities)
        {
            return capabilities.Any(capability => capability.Key == ClusterCrudCapabilitityName);
        }

        internal static IEnumerable<WabStorageAccountConfiguration> ResolveStorageAccounts(IEnumerable<WabStorageAccountConfiguration> storageAccounts)
        {
            return storageAccounts.Select(ResolveStorageAccount).ToList();
        }

        internal static WabStorageAccountConfiguration ResolveStorageAccount(WabStorageAccountConfiguration storageAccount)
        {
            return new WabStorageAccountConfiguration(
                GetFullyQualifiedStorageAccountName(storageAccount.Name), storageAccount.Key, storageAccount.Container);
        }

        internal static string GetFullyQualifiedStorageAccountName(string accountName)
        {
            // accountName
            if (accountName.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}.blob.core.windows.net", accountName);
            }

            return accountName;
        }

        public ILogger Logger { get; private set; }
    }
}