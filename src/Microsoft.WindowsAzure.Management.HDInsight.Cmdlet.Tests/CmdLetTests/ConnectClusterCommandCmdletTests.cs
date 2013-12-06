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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class ConnectClusterCommandCmdletTests : IntegrationTestBase
    {
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
        public void ICanCallThe_Connect_ClusterHDInsightClusterCmdlet()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.UseAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                IAzureHDInsightConnectionSessionManager sessionManager =
                    ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
                AzureHDInsightClusterConnection currentCluster = sessionManager.GetCurrentCluster();
                Assert.IsNotNull(currentCluster);
                ValidateGetCluster(currentCluster.Cluster);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Connect_ClusterHDInsightClusterCmdlet_MoreThanOnce()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IHDInsightCertificateCredential credentials = GetValidCredentials();
                IPipelineResult clusterResults =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                            .Invoke();
                IEnumerable<AzureHDInsightCluster> clusters = clusterResults.Results.ToEnumerable<AzureHDInsightCluster>();
                foreach (AzureHDInsightCluster cluster in clusters)
                {
                    IPipelineResult results =
                        runspace.NewPipeline()
                                .AddCommand(CmdletConstants.UseAzureHDInsightCluster)
                                .WithParameter(CmdletConstants.Subscription, cluster.SubscriptionId.ToString())
                                .WithParameter(CmdletConstants.Name, cluster.Name)
                                .Invoke();
                    Assert.AreEqual(1, results.Results.Count);
                    IAzureHDInsightConnectionSessionManager sessionManager =
                        ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
                    AzureHDInsightClusterConnection currentCluster = sessionManager.GetCurrentCluster();
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
        public void ICanCallThe_Connect_ClusterHDInsightClusterCmdlet_WithDebug()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.UseAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                            .WithParameter(CmdletConstants.Debug, null)
                            .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                IAzureHDInsightConnectionSessionManager sessionManager =
                    ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
                AzureHDInsightClusterConnection currentCluster = sessionManager.GetCurrentCluster();
                Assert.IsNotNull(currentCluster);
                string expectedLogMessage = "Getting hdinsight clusters for subscriptionid : " + TestCredentials.SubscriptionId;
                ValidateGetCluster(currentCluster.Cluster);
                Assert.IsTrue(logWriter.Buffer.Any(message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICannotCallThe_Connect_ClusterHDInsightClusterCmdlet_WithNonExistantCluster()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                string invalidCluster = Guid.NewGuid().ToString();
                string errorMessage = string.Empty;
                try
                {
                    IPipelineResult results =
                        runspace.NewPipeline()
                                .AddCommand(CmdletConstants.UseAzureHDInsightCluster)
                                .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                .WithParameter(CmdletConstants.Name, invalidCluster)
                                .Invoke();
                    Assert.Fail("The expected exception was not thrown.");
                }
                catch (CmdletInvocationException cmdException)
                {
                    errorMessage = cmdException.GetBaseException().Message;
                }

                Assert.AreEqual("Failed to connect to cluster :" + invalidCluster, errorMessage);
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private static void ValidateGetCluster(AzureHDInsightCluster expected, AzureHDInsightCluster actual)
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
            WabStorageAccountConfiguration defaultStorageAccount = GetWellKnownStorageAccounts().First();
            Assert.AreEqual(defaultStorageAccount.Key, cluster.DefaultStorageAccount.StorageAccountKey);
            Assert.AreEqual(defaultStorageAccount.Name, cluster.DefaultStorageAccount.StorageAccountName);
            Assert.AreEqual(defaultStorageAccount.Container, cluster.DefaultStorageAccount.StorageContainerName);
            foreach (WabStorageAccountConfiguration account in GetWellKnownStorageAccounts().Skip(1))
            {
                AzureHDInsightStorageAccount deserializedAccount =
                    cluster.StorageAccounts.FirstOrDefault(acc => acc.StorageAccountName == account.Name);
                Assert.IsNotNull(deserializedAccount, account.Name);
                Assert.AreEqual(account.Key, deserializedAccount.StorageAccountKey);
            }
        }
    }
}
