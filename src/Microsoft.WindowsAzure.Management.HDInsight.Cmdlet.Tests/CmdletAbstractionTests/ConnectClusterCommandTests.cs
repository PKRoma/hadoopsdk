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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class ConnectClusterCommandTests : IntegrationTestBase
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
        public void CanConnectToValidCluster()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            IUseAzureHDInsightClusterCommand connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
            connectCommand.Subscription = creds.SubscriptionId.ToString();
            connectCommand.Certificate = creds.Certificate;
            connectCommand.Name = TestCredentials.WellKnownCluster.DnsName;
            connectCommand.EndProcessing();

            Assert.AreEqual(1, connectCommand.Output.Count);
            AzureHDInsightClusterConnection currentCluster = connectCommand.Output.First();
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
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IHDInsightCertificateCredential creds = GetValidCredentials();
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightCluster)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Name, TestCredentials.WellKnownCluster.DnsName)
                            .Invoke();
                IEnumerable<AzureHDInsightCluster> clusters = results.Results.ToEnumerable<AzureHDInsightCluster>();
                foreach (AzureHDInsightCluster cluster in clusters)
                {
                    IUseAzureHDInsightClusterCommand connectCommand =
                        ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
                    connectCommand.Subscription = creds.SubscriptionId.ToString();
                    connectCommand.Certificate = creds.Certificate;
                    connectCommand.Name = cluster.Name;
                    connectCommand.EndProcessing();
                    Assert.AreEqual(1, connectCommand.Output.Count);
                    AzureHDInsightClusterConnection currentCluster = connectCommand.Output.First();
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            string errorMessage = string.Empty;
            string invalidCluster = Guid.NewGuid().ToString();
            try
            {
                IUseAzureHDInsightClusterCommand connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
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
            string errorMessage = string.Empty;
            string invalidCluster = "NoHttpAccessCluster";
            try
            {
                IUseAzureHDInsightClusterCommand connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
                connectCommand.Subscription = TestCredentials.SubscriptionId.ToString();
                connectCommand.Name = invalidCluster;
                connectCommand.EndProcessing().Wait();
            }
            catch (AggregateException aex)
            {
                errorMessage = aex.InnerExceptions.FirstOrDefault().Message;
            }

            string expectedErrorMessage = string.Format(
                CultureInfo.InvariantCulture,
                "Cluster {0} is not configured for Http Services access.\r\nPlease use the Grant Azure HDInsight Http Services Access cmdlet to enable Http Services access.",
                invalidCluster);

            Assert.AreEqual(expectedErrorMessage, errorMessage);
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
