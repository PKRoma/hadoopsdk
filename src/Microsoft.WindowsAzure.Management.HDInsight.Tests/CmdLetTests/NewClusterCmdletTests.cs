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
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;

    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Moq;

    [TestClass]
    public class NewClusterCmdletTests : IntegrationTestBase
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
        public void ICanCreateAClusterUsingPowerShellAndConfig()
        {
            var creds = GetValidCredentials();
            var coreConfig = new Hashtable();
            coreConfig.Add("hadoop.logfile.size", "10000");

            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                                      .WithParameter(CmdletConstants.CoreConfig, coreConfig)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Version, TestCredentials.WellKnownCluster.Version)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                Assert.AreEqual(1,
                                results.Results.Count);
                Assert.AreEqual(dnsName,
                                results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1,
                                getCommand.Output.Count);
                Assert.AreEqual(dnsName,
                                getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription,
                                                 creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate,
                                                 creds.Certificate)
                                  .WithParameter(CmdletConstants.Name,
                                                 dnsName)
                                  .WithParameter(CmdletConstants.Location,
                                                 CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected,
                                getCommand.Output.Count);
            }
        }
        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfig_WithDebug()
        {
            var creds = GetValidCredentials();
            var coreConfig = new Hashtable();
            coreConfig.Add("hadoop.logfile.size", "10000");

            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommandLogWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = getCommandLogWriter;
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Logger = getCommandLogWriter;
                getCommand.Certificate = creds.Certificate;
                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var expectedLogMessage = "Getting hdinsight clusters for subscriptionid : " + creds.SubscriptionId.ToString();
                Assert.IsTrue(
                    getCommandLogWriter.Buffer.Any(
                        message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();

                var newClusterCommandLogWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = newClusterCommandLogWriter;
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                                      .WithParameter(CmdletConstants.CoreConfig, coreConfig)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Debug, null)
                                      .WithParameter(CmdletConstants.Version, TestCredentials.WellKnownCluster.Version)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                Assert.AreEqual(1,
                                results.Results.Count);
                Assert.AreEqual(dnsName,
                                results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                expectedLogMessage = string.Format(CultureInfo.InvariantCulture, "Creating cluster '{0}' in location {1}", dnsName, CmdletConstants.EastUs);
                Assert.IsTrue(
                    newClusterCommandLogWriter.Buffer.Any(
                        message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1,
                                getCommand.Output.Count);
                Assert.AreEqual(dnsName,
                                getCommand.Output.ElementAt(0).Name);
                var deleteClusterCommandLogWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = deleteClusterCommandLogWriter;
                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription,
                                                 creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate,
                                                 creds.Certificate)
                                  .WithParameter(CmdletConstants.Name,
                                                 dnsName)
                                  .WithParameter(CmdletConstants.Location,
                                                 CmdletConstants.EastUs)
                                  .WithParameter(CmdletConstants.Debug,
                                                 null)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);
                expectedLogMessage = string.Format(CultureInfo.InvariantCulture, "Deleting cluster '{0}' in location {1}", dnsName, CmdletConstants.EastUs);
                Assert.IsTrue(
                    deleteClusterCommandLogWriter.Buffer.Any(
                        message => message.Contains(expectedLogMessage)));
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected,
                                getCommand.Output.Count);
                BufferingLogWriterFactory.Reset();
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfig_New_Set_Add_Hive_Oozie()
        {
            IHDInsightManagementPocoClientFactory pocoFactory = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>();
            var flowFactory = new TestPocoClientFactoryFlowThrough(pocoFactory);
            var manager = ServiceLocator.Instance.Locate<IServiceLocationIndividualTestManager>();
            manager.Override<IHDInsightManagementPocoClientFactory>(flowFactory);

            var creds = GetCredentials(TestCredentialsNames.Default);
            var certificate = new X509Certificate2(creds.Certificate);

            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;

                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, creds.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, creds.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, creds.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, creds.Environments[0].AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, creds.Environments[0].AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletConstants.SqlAzureServerName, creds.Environments[0].HiveStores[0].SqlServer)
                                      .WithParameter(CmdletConstants.DatabaseName, creds.Environments[0].HiveStores[0].Database)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(creds.AzureUserName, creds.AzurePassword))
                                      .WithParameter(CmdletConstants.MetastoreType, AzureHDInsightMetastoreType.HiveMetastore)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletConstants.SqlAzureServerName, creds.Environments[0].OozieStores[0].SqlServer)
                                      .WithParameter(CmdletConstants.DatabaseName, creds.Environments[0].OozieStores[0].Database)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(creds.AzureUserName, creds.AzurePassword))
                                      .WithParameter(CmdletConstants.MetastoreType, AzureHDInsightMetastoreType.OozieMetastore)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                var request = flowFactory.Clients.Where(c => c.LastCreateRequest.IsNotNull()).First().LastCreateRequest;
                Assert.IsNotNull(request.HiveMetastore);
                Assert.IsNotNull(request.OozieMetastore);

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfig_New_Set_Add_Hive_Oozie_And_CoreConfig()
        {
            IHDInsightManagementPocoClientFactory pocoFactory = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>();
            var flowFactory = new TestPocoClientFactoryFlowThrough(pocoFactory);
            var manager = ServiceLocator.Instance.Locate<IServiceLocationIndividualTestManager>();
            manager.Override<IHDInsightManagementPocoClientFactory>(flowFactory);

            var creds = GetCredentials(TestCredentialsNames.Default);
            var certificate = new X509Certificate2(creds.Certificate);

            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;

                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, creds.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, creds.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, creds.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, creds.Environments[0].AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, creds.Environments[0].AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletConstants.SqlAzureServerName, creds.Environments[0].HiveStores[0].SqlServer)
                                      .WithParameter(CmdletConstants.DatabaseName, creds.Environments[0].HiveStores[0].Database)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(creds.AzureUserName, creds.AzurePassword))
                                      .WithParameter(CmdletConstants.MetastoreType, AzureHDInsightMetastoreType.HiveMetastore)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletConstants.SqlAzureServerName, creds.Environments[0].OozieStores[0].SqlServer)
                                      .WithParameter(CmdletConstants.DatabaseName, creds.Environments[0].OozieStores[0].Database)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(creds.AzureUserName, creds.AzurePassword))
                                      .WithParameter(CmdletConstants.MetastoreType, AzureHDInsightMetastoreType.OozieMetastore)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                var request = flowFactory.Clients.Where(c => c.LastCreateRequest.IsNotNull()).First().LastCreateRequest;
                Assert.IsNotNull(request.HiveMetastore);
                Assert.IsNotNull(request.OozieMetastore);

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfigWithAnStorageAccountAfterTheSet()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfigWithAnStorageAccountBeforeTheSet()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                var expected = getCommand.Output.Count();

                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(expected, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanAddMultipleStorageAccountsUsingPowerShell()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var additionalStorageAccount = new WabStorageAccountConfiguration(
                    TestCredentials.Environments[0].AdditionalStorageAccounts[0].Name,
                    TestCredentials.Environments[0].AdditionalStorageAccounts[0].Key);
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, additionalStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, additionalStorageAccount.Key)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                var storageAccounts = getCommand.Output.Last().StorageAccounts.ToList();
                Assert.AreEqual(storageAccounts.Count, 1);

                var additionalStorageAccountFromOutput = storageAccounts.FirstOrDefault(acc => acc.StorageAccountName == additionalStorageAccount.Name);
                Assert.IsNotNull(additionalStorageAccountFromOutput);
                Assert.AreEqual(additionalStorageAccount.Key, additionalStorageAccountFromOutput.StorageAccountKey);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Name = dnsName;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(0, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterWithThreeOrMoreAsvAccountsUsingPowerShell()
        {
            var dnsName = this.GetRandomClusterName();
            var storageAccounts =
                IntegrationTestBase.TestCredentials.Environments.SelectMany(env => env.AdditionalStorageAccounts)
                                   .Select(acc => new WabStorageAccountConfiguration(acc.Name, acc.Key, acc.Container))
                                   .Take(3)
                                   .ToList();
            Assert.IsTrue(storageAccounts.Count >= 3);

            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightClusterConfig)
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletConstants.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletConstants.StorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.StorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.StorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                  .WithParameter(CmdletConstants.StorageAccountName, storageAccounts[0].Name)
                                  .WithParameter(CmdletConstants.StorageAccountKey, storageAccounts[0].Key)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                  .WithParameter(CmdletConstants.StorageAccountName, storageAccounts[1].Name)
                                  .WithParameter(CmdletConstants.StorageAccountKey, storageAccounts[1].Key)
                                      .AddCommand(CmdletConstants.AddAzureHDInsightStorage)
                                  .WithParameter(CmdletConstants.StorageAccountName, storageAccounts[2].Name)
                                  .WithParameter(CmdletConstants.StorageAccountKey, storageAccounts[2].Key)
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId)
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .Invoke();

                var clusterFromPowershell = results.Results.ToEnumerable<AzureHDInsightCluster>().First();
                var storageAccountsFromPowershell = clusterFromPowershell.StorageAccounts.ToList();
                Assert.AreEqual(3, storageAccountsFromPowershell.Count);

                foreach (var storageAccount in storageAccounts)
                {
                    var additionalStorageAccountFromOutput = storageAccountsFromPowershell.FirstOrDefault(acc => acc.StorageAccountName == storageAccount.Name);
                    Assert.IsNotNull(additionalStorageAccountFromOutput, "Storage account " + storageAccount.Name + " was not found");
                    Assert.AreEqual(storageAccount.Key, additionalStorageAccountFromOutput.StorageAccountKey);
                }
            }
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShell()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightCluster)
                    // Ensure that the subscription Id can be accepted as a string as well as a guid.
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Name, dnsName)
                                      .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                      .WithParameter(CmdletConstants.DefaultStorageAccountName, TestCredentials.Environments[0].DefaultStorageAccount.Name)
                                      .WithParameter(CmdletConstants.DefaultStorageAccountKey, TestCredentials.Environments[0].DefaultStorageAccount.Key)
                                      .WithParameter(CmdletConstants.DefaultStorageContainerName, TestCredentials.Environments[0].DefaultStorageAccount.Container)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential("hadoop", this.GetRandomValidPassword()))
                                      .WithParameter(CmdletConstants.ClusterSizeInNodes, 3)
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                  .WithParameter(CmdletConstants.Name, dnsName)
                                  .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                  .Invoke();

                Assert.AreEqual(0, results.Results.Count);

                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.Subscription = creds.SubscriptionId.ToString();
                getCommand.Name = dnsName;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(0, getCommand.Output.Count);
            }
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void ICannotDeleteANonExistantClusterUsingPowerShell()
        {
            var creds = GetValidCredentials();
            var invalidClusterName = Guid.NewGuid().ToString();
            using (var runspace = this.GetPowerShellRunspace())
            {

                try
                {
                    runspace.NewPipeline()
                                            .AddCommand(CmdletConstants.RemoveAzureHDInsightCluster)
                                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                            .WithParameter(CmdletConstants.Name, invalidClusterName)
                                            .WithParameter(CmdletConstants.Location, CmdletConstants.EastUs)
                                            .Invoke();
                    Assert.Fail("test failed");
                }
                catch (CmdletInvocationException invokeException)
                {
                    var invalidOperationException = invokeException.GetBaseException() as InvalidOperationException;
                    Assert.IsNotNull(invalidOperationException);
                    Assert.AreEqual("The cluster '" + invalidClusterName + "' doesn't exist.", invalidOperationException.Message);
                }
            }
        }
    }
}
