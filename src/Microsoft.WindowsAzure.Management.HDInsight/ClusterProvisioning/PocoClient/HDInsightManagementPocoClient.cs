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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class HDInsightManagementPocoClient : DisposableObject, IHDInsightManagementPocoClient
    {
        private readonly IConnectionCredentials credentials;

        internal HDInsightManagementPocoClient(IConnectionCredentials credentials)
        {
            this.credentials = credentials;
        }

        public async Task<Collection<ListClusterContainerResult>> ListContainers()
        {
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials))
            {
                string response = await client.ListCloudServices();
                return PayloadConverter.DeserializeListContainersResult(response, this.credentials.DeploymentNamespace);
            }
        }

        public async Task<ListClusterContainerResult> ListContainer(string dnsName)
        {
            var clusters = await this.ListContainers();
            return clusters.FirstOrDefault(cluster => cluster.DnsName.Equals(dnsName));
        }

        public void ValidateAsvAccounts(CreateClusterRequest request)
        {
            // Flats all the configurations into a single list for more uniform validation
            var asvList = request.AsvAccounts.Select(asv => new { account = asv.AccountName, key = asv.Key }).ToList();
            asvList.Add(new { account = request.DefaultAsvAccountName, key = request.DefaultAsvAccountKey });

            // Basic validation on the ASV configurations
            if (string.IsNullOrEmpty(request.DefaultAsvContainer))
            {
                throw new InvalidOperationException("Invalid Container. Default Storage Account Container cannot be null or empty");
            }
            if (asvList.Any(asv => string.IsNullOrEmpty(asv.account) || string.IsNullOrEmpty(asv.key)))
            {
                throw new InvalidOperationException("Invalid Azure Configuration. Credentials cannot be null or empty");
            }
            if (asvList.Any(asv => asv.account.Split(new char[] { '.' }).Length <= 1))
            {
                throw new InvalidOperationException("Invalid Azure Configuration. Azure Storage paths must contain the full address. e.g: foo.blob.core.windows.net");
            }
            if (asvList.GroupBy(asv => asv.account).Count(group => group.Count() > 1) > 0)
            {
                throw new InvalidOperationException("Invalid Azure Storage credentials. Duplicated values detected");
            }

            // Validates that we can stablish the connection to the ASV accounts and the default container
            var client = ServiceLocator.Instance.Locate<IAsvClientFactory>().Create();
            asvList.ForEach(asv => client.ValidateAccount(asv.account, asv.key).WaitForResult());
            client.ValidateContainer(request.DefaultAsvAccountName,
                                     request.DefaultAsvAccountKey,
                                     request.DefaultAsvContainer).WaitForResult();
        }

        public async Task CreateContainer(CreateClusterRequest request)
        {
            this.ValidateAsvAccounts(request);
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials))
            {
                string payload = PayloadConverter.SerializeClusterCreateRequest(request, this.credentials.SubscriptionId);
                await client.CreateContainer(request.DnsName, request.Location, payload);
            }
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
            await this.DeleteContainer(cluster.DnsName, cluster.Location);
        }

        public async Task DeleteContainer(string dnsName, string location)
        {
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(this.credentials))
            {
                await client.DeleteContainer(dnsName, location);
            }
        }

        public void WaitForClusterCondition(string dnsName, Func<ListClusterContainerResult, bool> evaluate, TimeSpan interval)
        {
            while (true)
            {
                var matchingContainer = this.ListContainer(dnsName).WaitForResult();
                if (evaluate(matchingContainer))
                {
                    return;
                }

                Thread.Sleep(interval);
            }
        }
    }
}