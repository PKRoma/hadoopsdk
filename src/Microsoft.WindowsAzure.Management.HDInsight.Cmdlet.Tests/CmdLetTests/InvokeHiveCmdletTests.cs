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
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class InvokeHiveCmdletTests : IntegrationTestBase
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
        public void CanCallTheExecHiveCmdlet()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ConnectToCluster(runspace, creds);

                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.InvokeHive)
                            .WithParameter(CmdletConstants.Query, "show tables")
                            .WithParameter(CmdletConstants.JobName, "show tables")
                            .Invoke();

                string outputContent = results.Results.ToEnumerable<string>().Last();
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ConnectToCluster(runspace, creds);

                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.InvokeHive)
                            .WithParameter(CmdletConstants.Query, "Fail this jobDetails.")
                            .WithParameter(CmdletConstants.JobName, "Fail this jobDetails.")
                            .Invoke();

                string outputContent = results.Results.ToEnumerable<string>().Last();
                Assert.AreEqual(5, results.Results.Count);
                Assert.AreEqual(AzureHDInsightJobSubmissionClientSimulator.JobFailed, outputContent);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CannotCallTheExecHiveCmdlet_WithoutConnecting()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                try
                {
                    IAzureHDInsightConnectionSessionManager sessionManager =
                        ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(null);
                    sessionManager.SetCurrentCluster(null);

                    runspace.NewPipeline().AddCommand(CmdletConstants.InvokeHive).WithParameter(CmdletConstants.Query, "show tables").Invoke();
                    Assert.Fail("test failed.");
                }
                catch (CmdletInvocationException cmdException)
                {
                    Assert.AreEqual(cmdException.ErrorRecord.CategoryInfo.Category, ErrorCategory.ConnectionError);
                }
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private static void ConnectToCluster(IRunspace runspace, IHDInsightCertificateCredential creds)
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
            ValidateGetCluster(currentCluster.Cluster);
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
