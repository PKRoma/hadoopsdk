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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class ConnectClusterCommandTests : IntegrationTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanConnectToValidCluster()
        {
            var creds = GetValidCredentials();
            var connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
            connectCommand.Subscription = creds.SubscriptionId.ToString();
            connectCommand.Certificate = creds.Certificate;
            connectCommand.Name = IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName;
            connectCommand.EndProcessing();

            Assert.AreEqual(1, connectCommand.Output.Count);
            var currentCluster = connectCommand.Output.First();
            Assert.IsNotNull(currentCluster);
            ValidateGetCluster(currentCluster.Cluster);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanConnectToValidClustersMoreThanOnce()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var credentials = IntegrationTestBase.GetValidCredentials();
                var client = HDInsightClient.Connect(new HDInsightCertificateCredential(credentials.SubscriptionId, credentials.Certificate));
                var clusters = client.ListClusters();
                foreach (var cluster in clusters)
                {
                    var connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
                    connectCommand.Subscription = creds.SubscriptionId.ToString();
                    connectCommand.Certificate = creds.Certificate;
                    connectCommand.Name = cluster.Name;
                    connectCommand.EndProcessing();
                    Assert.AreEqual(1, connectCommand.Output.Count);
                    var currentCluster = connectCommand.Output.First();
                    Assert.IsNotNull(currentCluster);
                    ValidateGetCluster(cluster, currentCluster.Cluster);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CannotConnectToInvalidCluster()
        {
            var creds = GetValidCredentials();
            string errorMessage = string.Empty;
            string invalidCluster = Guid.NewGuid().ToString();
            try
            {
                var connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
                connectCommand.Subscription = creds.SubscriptionId.ToString();
                connectCommand.Certificate = creds.Certificate;
                connectCommand.Name = invalidCluster;
                connectCommand.EndProcessing().Wait();
            }
            catch (AggregateException aex)
            {
                errorMessage = aex.InnerExceptions.FirstOrDefault().Message;
            }

            Assert.AreEqual("Failed to connect to cluster :" + invalidCluster, errorMessage);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CannotConnectToInvalidCluster_NoHttpAccess()
        {
            var httpAccessDisabledCluster = SyncClientScenarioTests.GetHttpAccessDisabledCluster();
            string errorMessage = string.Empty;
            string invalidCluster = Guid.NewGuid().ToString();
            try
            {
                var connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
                connectCommand.Subscription = httpAccessDisabledCluster.SubscriptionId.ToString();
                connectCommand.Name = httpAccessDisabledCluster.Name;
                connectCommand.EndProcessing().Wait();
            }
            catch (AggregateException aex)
            {
                errorMessage = aex.InnerExceptions.FirstOrDefault().Message;
            }

            string expectedErrorMessage = string.Format(
                CultureInfo.InvariantCulture,
                "Cluster {0} is not configured for Http Services access.\r\nPlease use the Grant Azure HDInsight Http Services Access cmdlet to enable Http Services access.",
                httpAccessDisabledCluster.Name);

            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        private static void ValidateGetCluster(ClusterDetails expected, AzureHDInsightCluster actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.HttpUserName, actual.HttpUserName);
            Assert.AreEqual(expected.HttpPassword, actual.HttpPassword);
        }

        private static void ValidateGetCluster(AzureHDInsightCluster cluster)
        {
            Assert.AreEqual(TestCredentials.WellKnownCluster.DnsName, cluster.Name);
            Assert.AreEqual(TestCredentials.WellKnownCluster.Version, cluster.Version);
            var defaultStorageAccount = IntegrationTestBase.GetWellKnownStorageAccounts().First();
            Assert.AreEqual(defaultStorageAccount.Key, cluster.DefaultStorageAccount.StorageAccountKey);
            Assert.AreEqual(defaultStorageAccount.Name, cluster.DefaultStorageAccount.StorageAccountName);
            Assert.AreEqual(defaultStorageAccount.Container, cluster.DefaultStorageAccount.StorageContainerName);
            foreach (var account in IntegrationTestBase.GetWellKnownStorageAccounts().Skip(1))
            {
                var deserializedAccount = cluster.StorageAccounts.FirstOrDefault(acc => acc.StorageAccountName == account.Name);
                Assert.IsNotNull(deserializedAccount, account.Name);
                Assert.AreEqual(account.Key, deserializedAccount.StorageAccountKey);
            }
        }
    }
}
