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
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class GetCommandCmdletTests : IntegrationTestBase
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
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .Invoke();

                IEnumerable<AzureHDInsightCluster> clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                AzureHDInsightCluster wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == TestCredentials.WellKnownCluster.DnsName);
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightCluster cluster = results.Results.ToEnumerable<AzureHDInsightCluster>().Last();
                ValidateGetCluster(cluster);
            }
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithDebug()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Debug, null)
                            .Invoke();
                IEnumerable<AzureHDInsightCluster> clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                AzureHDInsightCluster wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == TestCredentials.WellKnownCluster.DnsName);
                Assert.IsNotNull(wellKnownCluster);
                ValidateGetCluster(wellKnownCluster);

                string expectedLogMessage = "Getting hdinsight clusters for subscriptionid : " + TestCredentials.SubscriptionId;
                Assert.IsTrue(logWriter.Buffer.Any(message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithoutCertificate()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .Invoke();
                IEnumerable<AzureHDInsightCluster> clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                AzureHDInsightCluster wellKnownCluster = clusters.FirstOrDefault(cluster => cluster.Name == TestCredentials.WellKnownCluster.DnsName);
                Assert.IsNotNull(wellKnownCluster);
                ValidateGetCluster(wellKnownCluster);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICannotCallThe_Get_ClusterHDInsightClusterCmdlet_WithNonExistantCluster()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                try
                {
                    IPipelineResult results =
                        runspace.NewPipeline()
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

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
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
