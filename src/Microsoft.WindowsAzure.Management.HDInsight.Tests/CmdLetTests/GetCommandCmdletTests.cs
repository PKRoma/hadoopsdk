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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Net.Http;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Json;

    [TestClass]
    public class GetCommandCmdletTests : IntegrationTestBase
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
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .Invoke();

                var clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                var wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName);
                Assert.IsNotNull(wellKnownCluster);
                ValidateGetCluster(wellKnownCluster);
            }
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithDebug()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Debug, null)
                                      .Invoke();
                var clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                var wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName);
                Assert.IsNotNull(wellKnownCluster);
                ValidateGetCluster(wellKnownCluster);

                var expectedLogMessage = "Getting hdinsight clusters for subscriptionid : e4c4bcab-7e3b-4439-9919-d2e607f10286";
                Assert.IsTrue(
                    logWriter.Buffer.Any(
                        message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICannotCallThe_Get_ClusterHDInsightClusterCmdlet_WithNonExistantCluster()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                try
                {
                    var results = runspace.NewPipeline()
                                .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                                .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                .WithParameter(CmdletConstants.Name, Guid.NewGuid().ToString())
                                .Invoke();
                }
                catch (CmdletInvocationException cmdException)
                {
                    var baseHttpException = cmdException.GetBaseException() as HttpRequestException;
                    Assert.IsNotNull(baseHttpException, "expected httpException for invalid jobDetails id");
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithoutCertificate()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .Invoke();
                var clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                var wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName);
                Assert.IsNotNull(wellKnownCluster);
                ValidateGetCluster(wellKnownCluster);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithADnsName()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var cluster = results.Results.ToEnumerable<AzureHDInsightCluster>().Last();
                ValidateGetCluster(cluster);
            }
        }

        private static void ValidateGetCluster(AzureHDInsightCluster cluster)
        {
            Assert.AreEqual(TestCredentials.WellKnownCluster.DnsName, cluster.Name);
            Assert.AreEqual(TestCredentials.HadoopUserName, cluster.HttpUserName);
            Assert.AreEqual(TestCredentials.AzurePassword, cluster.HttpPassword);
            Assert.AreEqual(TestCredentials.SubscriptionId, cluster.SubscriptionId);
            Assert.AreEqual(ClusterState.Running, cluster.State);
            Assert.IsFalse(string.IsNullOrEmpty(cluster.ConnectionUrl));
            Assert.AreEqual(VersionStatus.Compatible.ToString(), cluster.VersionStatus);
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
