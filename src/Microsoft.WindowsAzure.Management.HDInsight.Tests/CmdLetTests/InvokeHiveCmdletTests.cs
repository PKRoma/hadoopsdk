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
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;

    [TestClass]
    public class InvokeHiveCmdletTests : IntegrationTestBase
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
        public void CanCallTheExecHiveCmdlet()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                ConnectToCluster(runspace, creds);

                var results = runspace.NewPipeline()
                    .AddCommand(CmdletConstants.InvokeHive)
                    .WithParameter(CmdletConstants.Query, "show tables")
                    .Invoke();

                var outputContent = results.Results.ToEnumerable<string>().Last();
                Assert.AreEqual(5, results.Results.Count);
                Assert.AreEqual("hivesampletable", outputContent);
            }
        }

        //[TestMethod]
        //[TestCategory("CheckIn")]
        //[TestCategory("Integration")]
        //[TestCategory("PowerShell")]
        //[TestCategory("Scenario")]
        //public void CanCallTheExecHiveCmdletAndCancelTheJob()
        //{
        //    var creds = GetValidCredentials();
        //    using (var runspace = this.GetPowerShellRunspace())
        //    {
        //        try
        //        {
        //            ConnectToCluster(runspace, creds);

        //            runspace.NewPipeline()
        //                    .AddCommand(CmdletConstants.InvokeHive)
        //                    .WithParameter(CmdletConstants.Query, "show tables")
        //                    .InvokeAndCancel();

        //        //var outputContent = results.Results.ToEnumerable<string>().Last();

        //        //    Assert.AreEqual(5, results.Results.Count);
        //        //    Assert.AreEqual("hivesampletable", outputContent);
        //        }
        //        finally
        //        {
        //            DisconnectFromCluster(runspace);
        //        }
        //    }
        //}

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanCallTheExecHiveCmdletWithInvalidQuery()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                ConnectToCluster(runspace, creds);

                var results = runspace.NewPipeline()
                    .AddCommand(CmdletConstants.InvokeHive)
                    .WithParameter(CmdletConstants.Query, "Fail this jobDetails.")
                    .Invoke();

                var outputContent = results.Results.ToEnumerable<string>().Last();
                Assert.AreEqual(5, results.Results.Count);
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
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                try
                {
                    var sessionManager = ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
                    sessionManager.SetCurrentCluster(null);

                    runspace.NewPipeline()
                              .AddCommand(CmdletConstants.InvokeHive)
                              .WithParameter(CmdletConstants.Query, "show tables")
                              .Invoke();
                    Assert.Fail("test failed.");
                }
                catch (CmdletInvocationException cmdException)
                {
                    Assert.AreEqual(cmdException.ErrorRecord.CategoryInfo.Category, ErrorCategory.ConnectionError);
                }
            }
        }

        private static void ConnectToCluster(
            IRunspace runspace, IHDInsightCertificateCredential creds)
        {
            var results =
                runspace.NewPipeline()
                        .AddCommand(CmdletConstants.UseAzureHDInsightCluster)
                        .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                        .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                        .WithParameter(CmdletConstants.Name, IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName)
                        .Invoke();
            Assert.AreEqual(1, results.Results.Count);
            var sessionManager = ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
            var currentCluster = sessionManager.GetCurrentCluster();
            ValidateGetCluster(currentCluster.Cluster);
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
