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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class InvokeHiveCommandTests : IntegrationTestBase
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
        public void CanCallTheExecHiveCommand()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                AzureHDInsightClusterConnection connection = ConnectToCluster(runspace, creds);
                IInvokeHiveCommand execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition { Query = "show tables", JobName = "show tables" };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                string outputContent = execHiveCommand.Output.Last();

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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                AzureHDInsightClusterConnection connection = ConnectToCluster(runspace, creds);
                IInvokeHiveCommand execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition { Query = "show tableaus", JobName = "Fail_this_job" };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                string outputContent = execHiveCommand.Output.Last();
                Assert.AreEqual(5, execHiveCommand.Output.Count());
                Assert.AreEqual(AzureHDInsightJobSubmissionClientSimulator.JobFailed, outputContent);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanCallTheExecHiveCommand_DoesNotUploadFile()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                AzureHDInsightClusterConnection connection = ConnectToCluster(runspace, creds);
                var storageHandlerSimulator = new AzureHDInsightStorageHandlerSimulator();
                AzureHDInsightStorageHandlerSimulatorFactory.Instance = storageHandlerSimulator;

                IInvokeHiveCommand execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition { File = "query.hql", JobName = "show tables" };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                string outputContent = execHiveCommand.Output.Last();
                Assert.AreEqual(5, execHiveCommand.Output.Count());
                Assert.AreEqual("hivesampletable", outputContent);

                Assert.IsNull(storageHandlerSimulator.UploadedStream);
                Assert.IsNull(storageHandlerSimulator.Path);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanCallTheExecHiveCommand_UploadsFile()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                AzureHDInsightClusterConnection connection = ConnectToCluster(runspace, creds);
                var storageHandlerSimulator = new AzureHDInsightStorageHandlerSimulator();
                AzureHDInsightStorageHandlerSimulatorFactory.Instance = storageHandlerSimulator;

                IInvokeHiveCommand execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition { Query = "show tables", JobName = "show tables" };
                execHiveCommand.Connection = connection;
                execHiveCommand.EndProcessing();

                string destinationPath = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://{0}/{1}/user/{2}/",
                    connection.Cluster.DefaultStorageAccount.StorageAccountName,
                    connection.Cluster.DefaultStorageAccount.StorageContainerName,
                    connection.Cluster.HttpUserName);
                string outputContent = execHiveCommand.Output.Last();
                Assert.IsNotNull(storageHandlerSimulator.UploadedStream);
                storageHandlerSimulator.UploadedStream.Seek(0, SeekOrigin.Begin);
                string contents = new StreamReader(storageHandlerSimulator.UploadedStream).ReadToEnd();
                Assert.AreEqual("show tables", contents);
                Assert.IsFalse(string.IsNullOrEmpty(storageHandlerSimulator.Path.OriginalString));
                Assert.IsTrue(storageHandlerSimulator.Path.OriginalString.StartsWith(destinationPath, StringComparison.OrdinalIgnoreCase));

                Assert.AreEqual(5, execHiveCommand.Output.Count());
                Assert.AreEqual("hivesampletable", outputContent);
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
                IInvokeHiveCommand execHiveCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateInvokeHive();
                execHiveCommand.JobDefinition = new AzureHDInsightHiveJobDefinition { Query = "show tables" };
                execHiveCommand.Connection = null;
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(argumentNullException.ParamName, "Connection");
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private static AzureHDInsightClusterConnection ConnectToCluster(IRunspace runspace, IHDInsightCertificateCredential creds)
        {
            IUseAzureHDInsightClusterCommand connectCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateUseCluster();
            connectCommand.Subscription = creds.SubscriptionId.ToString();
            connectCommand.Certificate = creds.Certificate;
            connectCommand.Name = TestCredentials.WellKnownCluster.DnsName;
            connectCommand.EndProcessing();
            Assert.AreEqual(1, connectCommand.Output.Count);
            AzureHDInsightClusterConnection currentCluster = connectCommand.Output.First();
            ValidateGetCluster(currentCluster.Cluster);
            return currentCluster;
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
