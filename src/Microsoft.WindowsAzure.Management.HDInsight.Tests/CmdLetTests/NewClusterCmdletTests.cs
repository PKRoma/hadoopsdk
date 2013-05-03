namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    [TestClass]
    public class NewClusterCmdletTests : IntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfig()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightConfig)
                                      .WithParameter(CmdletHardCodes.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletHardCodes.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, TestCredentials.DefaultStorageAccount.Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, TestCredentials.DefaultStorageAccount.Key)
                                      .WithParameter(CmdletHardCodes.StorageContainerName, TestCredentials.DefaultStorageAccount.Container)
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightCluster)
                                       // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId)
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .WithParameter(CmdletHardCodes.Name, dnsName)
                                      .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                      .WithParameter(CmdletHardCodes.UserName, "hadoop")
                                      .WithParameter(CmdletHardCodes.Password, this.GetRandomValidPassword())
                                      .Invoke();

                Assert.AreEqual(1,
                                results.Results.Count);
                Assert.AreEqual(dnsName,
                                results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1,
                                getCommand.Output.Count);
                Assert.AreEqual(dnsName,
                                getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletHardCodes.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletHardCodes.SubscriptionId,
                                                 creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletHardCodes.Certificate,
                                                 creds.Certificate)
                                  .WithParameter(CmdletHardCodes.Name,
                                                 dnsName)
                                  .WithParameter(CmdletHardCodes.Location,
                                                 CmdletHardCodes.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(1,
                                getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfig_New_Set_Add_Hive_Oozie()
        {
            IHDInsightManagementPocoClientFactory pocoFactory = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>();
            var flowFactory = new TestPocoClientFactoryFlowThrough(pocoFactory);
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();
            manager.Override<IHDInsightManagementPocoClientFactory>(flowFactory);

            var creds = this.GetCredentials("default");
            var certificate = new X509Certificate2(creds.Certificate);

            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightConfig)
                                      .WithParameter(CmdletHardCodes.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletHardCodes.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, creds.DefaultStorageAccount.Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, creds.DefaultStorageAccount.Key)
                                      .WithParameter(CmdletHardCodes.StorageContainerName, creds.DefaultStorageAccount.Container)
                                      .AddCommand(CmdletHardCodes.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, creds.AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, creds.AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletHardCodes.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletHardCodes.SqlAzureServerName, creds.HiveStores[0].SqlServer)
                                      .WithParameter(CmdletHardCodes.DatabaseName, creds.HiveStores[0].Database)
                                      .WithParameter(CmdletHardCodes.UserName, creds.HiveStores[0].Username)
                                      .WithParameter(CmdletHardCodes.Password, creds.HiveStores[0].Password)
                                      .WithParameter(CmdletHardCodes.MetastoreType, AzureHDInsightMetastoreType.HiveMetastore)
                                      .AddCommand(CmdletHardCodes.AddAzureHDInsightMetastore)
                                      .WithParameter(CmdletHardCodes.SqlAzureServerName, creds.OozieStores[0].SqlServer)
                                      .WithParameter(CmdletHardCodes.DatabaseName, creds.OozieStores[0].Database)
                                      .WithParameter(CmdletHardCodes.UserName, creds.OozieStores[0].Username)
                                      .WithParameter(CmdletHardCodes.Password, creds.OozieStores[0].Password)
                                      .WithParameter(CmdletHardCodes.MetastoreType, AzureHDInsightMetastoreType.OozieMetastore)
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightCluster)
                                      // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId)
                                      .WithParameter(CmdletHardCodes.Certificate, certificate)
                                      .WithParameter(CmdletHardCodes.Name, dnsName)
                                      .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                      .WithParameter(CmdletHardCodes.UserName, "hadoop")
                                      .WithParameter(CmdletHardCodes.Password, this.GetRandomValidPassword())
                                      .Invoke();

                var request = flowFactory.Clients.Where(c => c.LastCreateRequest.IsNotNull()).First().LastCreateRequest;
                Assert.IsNotNull(request.HiveMetastore);
                Assert.IsNotNull(request.OozieMetastore);

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletHardCodes.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                  .WithParameter(CmdletHardCodes.Name, dnsName)
                                  .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfigWithAnStorageAccountAfterTheSet()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightConfig)
                                      .WithParameter(CmdletHardCodes.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletHardCodes.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, TestCredentials.DefaultStorageAccount.Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, TestCredentials.DefaultStorageAccount.Key)
                                      .WithParameter(CmdletHardCodes.StorageContainerName, TestCredentials.DefaultStorageAccount.Container)
                                      .AddCommand(CmdletHardCodes.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, TestCredentials.AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, TestCredentials.AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightCluster)
                                      // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId)
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .WithParameter(CmdletHardCodes.Name, dnsName)
                                      .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                      .WithParameter(CmdletHardCodes.UserName, "hadoop")
                                      .WithParameter(CmdletHardCodes.Password, this.GetRandomValidPassword())
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletHardCodes.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                  .WithParameter(CmdletHardCodes.Name, dnsName)
                                  .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerShellAndConfigWithAnStorageAccountBeforeTheSet()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightConfig)
                                      .WithParameter(CmdletHardCodes.ClusterSizeInNodes, 3)
                                      .AddCommand(CmdletHardCodes.AddAzureHDInsightStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, TestCredentials.AdditionalStorageAccounts[0].Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, TestCredentials.AdditionalStorageAccounts[0].Key)
                                      .AddCommand(CmdletHardCodes.SetAzureHDInsightDefaultStorage)
                                      .WithParameter(CmdletHardCodes.StorageAccountName, TestCredentials.DefaultStorageAccount.Name)
                                      .WithParameter(CmdletHardCodes.StorageAccountKey, TestCredentials.DefaultStorageAccount.Key)
                                      .WithParameter(CmdletHardCodes.StorageContainerName, TestCredentials.DefaultStorageAccount.Container)
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightCluster)
                                      // Ensure that the subscription Id can be accepted as a guid as well as a string.
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId)
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .WithParameter(CmdletHardCodes.Name, dnsName)
                                      .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                      .WithParameter(CmdletHardCodes.UserName, "hadoop")
                                      .WithParameter(CmdletHardCodes.Password, this.GetRandomValidPassword())
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletHardCodes.RemoveAzureHDInsightCluster)
                    // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                  .WithParameter(CmdletHardCodes.Name, dnsName)
                                  .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                  .Invoke();

                Assert.AreEqual(0,
                                results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCreateAClusterUsingPowerSHell()
        {
            var creds = GetValidCredentials();
            var dnsName = this.GetRandomClusterName();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.NewAzureHDInsightCluster)
                                      // Ensure that the subscription Id can be accepted as a string as well as a guid.
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .WithParameter(CmdletHardCodes.Name, dnsName)
                                      .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                      .WithParameter(CmdletHardCodes.DefaultStorageAccountName, TestCredentials.DefaultStorageAccount.Name)
                                      .WithParameter(CmdletHardCodes.DefaultStorageAccountKey, TestCredentials.DefaultStorageAccount.Key)
                                      .WithParameter(CmdletHardCodes.DefaultStorageContainerName, TestCredentials.DefaultStorageAccount.Container)
                                      .WithParameter(CmdletHardCodes.UserName, "hadoop")
                                      .WithParameter(CmdletHardCodes.Password, this.GetRandomValidPassword())
                                      .WithParameter(CmdletHardCodes.ClusterSizeInNodes, 3)
                                      .Invoke();

                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(dnsName, results.Results.ElementAt(0).ImmediateBaseObject.CastTo<AzureHDInsightCluster>().Name);

                var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;
                getCommand.Name = dnsName;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
                Assert.AreEqual(dnsName, getCommand.Output.ElementAt(0).Name);

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletHardCodes.RemoveAzureHDInsightCluster)
                                  // Ensure that subscription id can be accepted as a sting as well as a guid.
                                  .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                  .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                  .WithParameter(CmdletHardCodes.Name, dnsName)
                                  .WithParameter(CmdletHardCodes.Location, CmdletHardCodes.EastUs)
                                  .Invoke();

                Assert.AreEqual(0, results.Results.Count);


                getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
                getCommand.SubscriptionId = creds.SubscriptionId;
                getCommand.Certificate = creds.Certificate;

                getCommand.EndProcessing();
                Assert.AreEqual(1, getCommand.Output.Count);
            }
        }
    }
}
