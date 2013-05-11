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

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.ConnectionCredentials;

    [TestClass]
    public class SyncClientScenarioTests : IntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }
        
        [TestMethod]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("Scenario")]
        [TestCategory("LongRunning")]
        [Timeout(35 * 60 * 1000)] // ms
        public void CreateDeleteContainer_SyncClientWithTimeouts_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            CreateDeleteContainer_SyncClientWithTimeouts();
        }

        [TestMethod]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("Scenario")]
        [TestCategory("LongRunning")]
        [Timeout(35 * 60 * 1000)] // ms
        public void CreateDeleteContainer_BasicClusterAsyncClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            CreateDeleteContainer_BasicClusterAsyncClient();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        public void CreateDeleteContainer_SyncClientWithTimeouts()
        {
            // Creates the client
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);
            TestValidAdvancedCluster(
                client.ListClusters,
                client.GetCluster,
                cluster => client.CreateCluster(cluster, TimeSpan.FromMinutes(25)),
                dnsName => client.DeleteCluster(dnsName, TimeSpan.FromMinutes(5)));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        public void CreateDeleteContainer_SyncClient()
        {
            // Creates the client
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);
            TestValidAdvancedCluster(
                client.ListClusters,
                client.GetCluster,
                client.CreateCluster,
                client.DeleteCluster);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        public void CreateDeleteContainer_AsyncClient()
        {
            // Creates the client
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);

            TestValidAdvancedCluster(
                () => client.ListClustersAsync().WaitForResult(),
                dnsName => client.GetClusterAsync(dnsName).WaitForResult(),
                cluster => client.CreateClusterAsync(cluster).WaitForResult(),
                dnsName => client.DeleteClusterAsync(dnsName).WaitForResult());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        public void CreateDeleteContainer_BasicClusterAsyncClient()
        {
            // Creates the client
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);

            TestClusterEndToEnd(
                base.GetRandomCluster(),
                () => client.ListClustersAsync().WaitForResult(),
                dnsName => client.GetClusterAsync(dnsName).WaitForResult(),
                cluster => client.CreateClusterAsync(cluster).WaitForResult(),
                dnsName => client.DeleteClusterAsync(dnsName).WaitForResult());
        }

        [TestMethod]
        [TestCategory("Manual")]
        [TestCategory("Scenario")]
        [TestCategory("LongRunning")]
        [Timeout(30 * 60 * 1000)]  // ms
        public void CreateDeleteContainer_SyncClient_AgainstManualEnvironment()
        {
            // Dissables the simulator
            this.ApplyIndividualTestMockingOnly();

            // Sets the simulator
            var runManager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();
            runManager.Override<IConnectionCredentialsFactory>(new AlternativeEnvironmentConnectionCredentialsFactory());

            // Creates the client
            if (IntegrationTestBase.TestCredentials.AlternativeEnvironment == null)
                Assert.Inconclusive("Alternative Azure Endpoint wasn't set up");
            var client = new ClusterProvisioningClient(
                IntegrationTestBase.TestCredentials.AlternativeEnvironment.SubscriptionId,
                IntegrationTestBase.GetValidCredentials().Certificate);
            client.PollingInterval = TimeSpan.FromSeconds(1);

            // Runs the test
            TestValidAdvancedCluster(
                client.ListClusters,
                client.GetCluster,
                client.CreateCluster,
                client.DeleteCluster);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        [ExpectedException(typeof(OperationCanceledException))]
        public void InvalidCreateDeleteContainer_FailsOnSdk_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            InvalidCreateDeleteContainer_FailsOnSdk();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        [ExpectedException(typeof(OperationCanceledException))]
        public void InvalidCreateDeleteContainer_FailsOnSdk()
        {            
            var clusterRequest = base.GetRandomCluster();
            clusterRequest.HiveMetastore = new HDInsightMetastore(TestCredentials.HiveStores[0].SqlServer,
                                                           TestCredentials.HiveStores[0].Database,
                                                           TestCredentials.HiveStores[0].UserName,
                                                           TestCredentials.HiveStores[0].Password);
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);

            TestClusterEndToEnd(
                clusterRequest,
                () => client.ListClustersAsync().WaitForResult(),
                dnsName => client.GetClusterAsync(dnsName).WaitForResult(),
                cluster => client.CreateClusterAsync(cluster).WaitForResult(),
                dnsName => client.DeleteClusterAsync(dnsName).WaitForResult());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidCreateDeleteContainer_FailsOnServer_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            InvalidCreateDeleteContainer_FailsOnServer();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Scenario")]
        [Timeout(30 * 1000)] // ms
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidCreateDeleteContainer_FailsOnServer()
        {
            var clusterRequest = base.GetRandomCluster();
            clusterRequest.AdditionalStorageAccounts.Add(new StorageAccountConfiguration("invalid", TestCredentials.AdditionalStorageAccounts[0].Key));

            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            client.PollingInterval = TimeSpan.FromMilliseconds(100);

            TestClusterEndToEnd( 
                clusterRequest,
                client.ListClusters,
                client.GetCluster,
                client.CreateCluster,
                client.DeleteCluster);
        }
    
        private void TestValidAdvancedCluster(
            Func<Collection<HDInsightCluster>> getClusters,
            Func<string, HDInsightCluster> getCluster,
            Func<HDInsightClusterCreationDetails, HDInsightCluster> createCluster,
            Action<string> deleteCluster)
        {
            // ClusterName
            var cluster = base.GetRandomCluster();
            cluster.AdditionalStorageAccounts.Add(new StorageAccountConfiguration(TestCredentials.AdditionalStorageAccounts[0].Name, TestCredentials.AdditionalStorageAccounts[0].Key));
            cluster.OozieMetastore = new HDInsightMetastore(TestCredentials.OozieStores[0].SqlServer,
                                                            TestCredentials.OozieStores[0].Database,
                                                            TestCredentials.OozieStores[0].UserName,
                                                            TestCredentials.OozieStores[0].Password);
            cluster.HiveMetastore = new HDInsightMetastore(TestCredentials.HiveStores[0].SqlServer,
                                                           TestCredentials.HiveStores[0].Database,
                                                           TestCredentials.HiveStores[0].UserName,
                                                           TestCredentials.HiveStores[0].Password);

            this.TestClusterEndToEnd(cluster, getClusters, getCluster, createCluster, deleteCluster);
        }


        private void TestClusterEndToEnd(
            HDInsightClusterCreationDetails cluster,
            Func<Collection<HDInsightCluster>> getClusters,
            Func<string, HDInsightCluster> getCluster,
            Func<HDInsightClusterCreationDetails, HDInsightCluster> createCluster,
            Action<string> deleteCluster)
        {
            // TODO: DROP ALL THE TABLES IN THE METASTORE TABLES

            // Verifies it doesn't exist
            var listResult = getClusters();
            int matchingContainers = listResult.Count(container => container.Name.Equals(cluster.Name));
            Assert.AreEqual(0, matchingContainers);

            // Creates the cluster
            var result = createCluster(cluster);
            Assert.IsNotNull(result);
            Assert.IsNotNull(getCluster(cluster.Name));

            // TODO: USE HADOOP SDK TO LAUNCH A JOB USING BOTH STORAGE ACCOUNTS

            // TODO: QUERY SQL METASTORES TO SEE THAT THE DATABASES GOT INITIALIZED

            // Deletes the cluster
            deleteCluster(cluster.Name);

            // Verifies it doesn't exist
            Assert.IsNull(getCluster(cluster.Name));
        }

        // Negative tests mocking the Create\List\Delete poco layers and making sure I don't get AggregateExceptions with WaitForResult or await

        // Negative tests mocking the Create\List\Delete poco layers and making sure I don't get AggregateExceptions

    }
}