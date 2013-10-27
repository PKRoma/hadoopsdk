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
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;

    [TestClass]
    public class InvokeHiveCommandTests : IntegrationTestBase
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
        public void CanCallTheExecHiveCommand()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var connection = ConnectToCluster(runspace, creds);
                var execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition() { Query = "show tables" };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                var outputContent = execHiveCommand.Output.Last();

                Assert.AreEqual(5, execHiveCommand.Output.Count());
                Assert.AreEqual("hivesampletable", outputContent);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanCallTheExecHiveCommandWithAFailedQuery()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var connection = ConnectToCluster(runspace, creds);
                var execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition()
                {
                    Query = "show tableaus",
                    JobName = "Fail_this_job"
                };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                var outputContent = execHiveCommand.Output.Last();
                Assert.AreEqual(5, execHiveCommand.Output.Count());
                Assert.AreEqual(HadoopJobSubmissionPocoSimulatorClient.JobFailed, outputContent);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CannotCallTheExecHiveCmdlet_WithoutConnecting()
        {
            try
            {
                var execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition() { Query = "show tables" };
                execHiveCommand.Connection = null;
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(argumentNullException.ParamName, "Connection");
            }
        }

        private static AzureHDInsightClusterConnection ConnectToCluster(
            IRunspace runspace, IHDInsightCertificateCredential creds)
        {
            var connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
            connectCommand.Subscription = creds.SubscriptionId.ToString();
            connectCommand.Certificate = creds.Certificate;
            connectCommand.Name = IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName;
            connectCommand.EndProcessing();
            Assert.AreEqual(1, connectCommand.Output.Count);
            var currentCluster = connectCommand.Output.First();
            ValidateGetCluster(currentCluster.Cluster);
            return currentCluster;
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
