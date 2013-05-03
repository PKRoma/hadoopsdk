namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    [TestClass]
    public class PocoClientTests : IntegrationTestBase
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
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanPerformA_ListContainders_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_ListContainers_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task ICanPerformA_ListContainers_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials))
            {
                var containers =
                    from container in await client.ListContainers()
                    where container.DnsName.Equals(TestCredentials.DnsName)
                    select container;
                Assert.AreEqual(1, containers.Count());

                var result = await client.ListContainer(TestCredentials.DnsName);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Location, "East US");
                Assert.AreEqual(result.UserName, "sa-po-svc");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanPerformA_ListNonExistingContainer_Using_PocoClientAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_ListNonExistingContainer_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task ICanPerformA_ListNonExistingContainer_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials))
            {
                var result = await client.ListContainer(base.GetRandomClusterName());
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ICanPerformA_DeleteNonExistingContainer_Using_PocoClientAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_DeleteNonExistingContainer_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ICanPerformA_DeleteNonExistingContainer_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>()
                                   .Create(credentials)
                                   .DeleteContainer(base.GetRandomClusterName());
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [ExpectedException(typeof(HDInsightRestClientException))]
        public async Task ICanPerformA_DeleteNonExistingContainerInternal_Using_PocoClientAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_DeleteNonExistingContainerInternal_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(HDInsightRestClientException))]
        public async Task ICanPerformA_DeleteNonExistingContainerInternal_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>()
                                   .Create(credentials)
                                   .DeleteContainer(base.GetRandomClusterName(), "East US");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(HDInsightRestClientException))]
        public async Task ICanPerformA_DeleteContainerInvalidLocationInternal_Using_PocoClientAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_DeleteContainerInvalidLocationInternal_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(HDInsightRestClientException))]
        public async Task ICanPerformA_DeleteContainerInvalidLocationInternal_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>()
                                   .Create(credentials)
                                   .DeleteContainer(TestCredentials.DnsName, "Nowhere");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_DeleteContainer_WithAnInvalidCertificate_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_DeleteContainer_WithAnInvalidCertificate_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_DeleteContainer_WithAnInvalidCertificate_PocoClientAbstraction()
        {
            var creds = GetInvalidCertificateCredentails();
            var clusterName = TestCredentials.DnsName;
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.DeleteContainer(clusterName);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_GetContainer_WithAnInvalidCertificate_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_GetContainer_WithAnInvalidCertificate_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_GetContainer_WithAnInvalidCertificate_PocoClientAbstraction()
        {
            var creds = GetInvalidCertificateCredentails();
            var clusterName = TestCredentials.DnsName;
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.ListContainer(clusterName);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_CreateCreateContainer_WithAnInvalidCertificate_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_CreateCreateContainer_WithAnInvalidCertificate_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_CreateCreateContainer_WithAnInvalidCertificate_PocoClientAbstraction()
        {
            var creds = GetInvalidCertificateCredentails();
            var cluster = this.GetRandomCluster();
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.CreateContainer(cluster);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_DeleteContainer_WithAnInvalidSubscriptionId_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_DeleteContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_DeleteContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction()
        {
            var creds = GetInvalidSubscriptionIdCredentails();
            var clusterName = TestCredentials.DnsName;
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.DeleteContainer(clusterName);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_GetContainer_WithAnInvalidSubscriptionId_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_GetContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_GetContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction()
        {
            var creds = GetInvalidSubscriptionIdCredentails();
            var clusterName = TestCredentials.DnsName;
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.ListContainer(clusterName);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task ICanNotPerformA_CreateCreateContainer_WithAnInvalidSubscriptionId_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.ICanNotPerformA_CreateCreateContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("NegitiveTest")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanNotPerformA_CreateCreateContainer_WithAnInvalidSubscriptionId_PocoClientAbstraction()
        {
            var creds = GetInvalidSubscriptionIdCredentails();
            var cluster = this.GetRandomCluster();
            try
            {
                using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(creds))
                {
                    await client.CreateContainer(cluster);
                }
                Assert.Fail("This call was expected to receive a failure, but did not.");
            }
            catch (HDInsightRestClientException ex)
            {
                Assert.IsTrue(ex.RequestStatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public void ICanPerformA_BasicCreateDeleteContainers_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            ICanPerformA_BasicCreateDeleteContainers_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public void ICanPerformA_BasicCreateDeleteContainers_Using_PocoClientAbstraction()
        {
            var cluster = base.GetRandomCluster();
            ValidateCreateClusterSucceeds(cluster);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public void ICanPerformA_AdvancedCreateDeleteContainers_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            ICanPerformA_AdvancedCreateDeleteContainers_Using_PocoClient();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public void ICanPerformA_AdvancedCreateDeleteContainers_Using_PocoClient()
        {
            var cluster = base.GetRandomCluster();
            cluster.AsvAccounts.Add(new AsvAccountConfiguration(TestCredentials.AdditionalStorageAccounts[0].Name,
                                                                TestCredentials.AdditionalStorageAccounts[0].Key));
            cluster.OozieMetastore = new ComponentMetastore(TestCredentials.OozieStores[0].SqlServer,
                                                            TestCredentials.OozieStores[0].Database,
                                                            TestCredentials.OozieStores[0].Username,
                                                            TestCredentials.OozieStores[0].Password);
            cluster.HiveMetastore = new ComponentMetastore(TestCredentials.HiveStores[0].SqlServer,
                                                           TestCredentials.HiveStores[0].Database,
                                                           TestCredentials.HiveStores[0].Username,
                                                           TestCredentials.HiveStores[0].Password);

            ValidateCreateClusterSucceeds(cluster);
        }

        private void ValidateCreateClusterSucceeds(CreateClusterRequest cluster)
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials))
            {
                client.CreateContainer(cluster);
                client.WaitForClusterCondition(
                    cluster.DnsName,
                    c => c != null && (this.CreatingStates.Contains(c.ParsedState) || c.ResultError != null),
                    TimeSpan.FromSeconds(1));

                var task = client.ListContainer(cluster.DnsName);
                task.WaitForResult();
                var container = task.Result;
                Assert.IsNotNull(container);
                Assert.IsNull(container.ResultError);

                client.DeleteContainer(cluster.DnsName);
                client.CreateContainer(cluster);
                client.WaitForClusterCondition(
                    cluster.DnsName,
                    c => c != null && (this.CreatingStates.Contains(c.ParsedState) || c.ResultError != null),
                    TimeSpan.FromMilliseconds(100));
            }
        }

        private readonly ClusterState[] CreatingStates = new ClusterState
            []
        {
            ClusterState.AzureVMConfiguration,
            ClusterState.ClusterStorageProvisioned,
            ClusterState.HDInsightConfiguration,
            ClusterState.Operational,
            ClusterState.Running,
        };

        //Task<ListClusterContainerResult> ListContainer(string dnsName);
        //Task<ListClusterContainerResult> ListContainer(string dnsName, string location);
        //Task DeleteContainer(string dnsName);

        // Delete Cluster wrong locations
        // Delete Cluster wrong name, right location
        // Delete non existing cluster

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidAsvConfig_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await NegativeTest_InvalidAsvConfig_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidAsvConfig_Using_PocoClientAbstraction()
        {
            var cluster = base.GetRandomCluster();
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            cluster.DefaultAsvAccountKey = "invalid";

            try
            {
                await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
            }
            catch (ConfigurationErrorsException e)
            {
                // NEIN: This needs to validate at least one parameter of the exception.
                Console.WriteLine("THIS TEST SUCCEDED because the expected negative result was found");
                Console.WriteLine("ASV Validation failed. Details: {0}", e.ToString());
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Expected exception 'ConfigurationErrorsException'; found '{0}'. Message:{1}", e.GetType(), e.Message);
            }

            Assert.Fail("ASV Validation should have failed.");

            try
            {
                await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
            }
            catch (AggregateException e)
            {
                Assert.IsNotNull(e.InnerException);
                Assert.IsNotNull(e.InnerException as ConfigurationErrorsException);
                Console.WriteLine("THIS TEST SUCCEDED because the expected negative result was found");
                Console.WriteLine("ASV Validation failed. Details: {0}", e.ToString());
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Expected exception 'ConfigurationErrorsException'; found '{0}'. Message:{1}", e.GetType(), e.Message);
            }

            Assert.Fail("ASV Validation should have failed.");
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidAsvContainer_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await NegativeTest_InvalidAsvContainer_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidAsvContainer_Using_PocoClientAbstraction()
        {
            var cluster = base.GetRandomCluster();
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            cluster.DefaultAsvContainer = Guid.NewGuid().ToString("N").ToLowerInvariant();

            try
            {
                await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
            }
            catch (ConfigurationErrorsException e)
            {
                // NEIN: This needs to validate at least one parameter of the exception.
                Console.WriteLine("THIS TEST SUCCEDED because the expected negative result was found");
                Console.WriteLine("ASV Validation failed. Details: {0}", e.ToString());
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Expected exception 'ConfigurationErrorsException'; found '{0}'. Message:{1}", e.GetType(), e.Message);
            }

            Assert.Fail("ASV Validation should have failed.");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_RepeatedAsvConfig_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.NegativeTest_RepeatedAsvConfig_Using_PocoClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_RepeatedAsvConfig_Using_PocoClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var cluster = this.GetRandomCluster();
            cluster.AsvAccounts.Add(new AsvAccountConfiguration(cluster.DefaultAsvAccountName, cluster.DefaultAsvAccountKey));

            try
            {
                await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("THIS TEST SUCCEDED because the expected negative result was found");
                Console.WriteLine("ASV Validation failed. Details: {0}", e.ToString());
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Expected exception 'InvalidOperationException'; found '{0}'. Message:{1}", e.GetType(), e.Message);
            }

            Assert.Fail("ASV Validation should have failed.");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task NegativeTest_InvalidAsv_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.NegativeTest_InvalidAsv_Using_PocoClient();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task NegativeTest_InvalidAsv_Using_PocoClient()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var cluster = base.GetRandomCluster();
            cluster.AsvAccounts.Add(new AsvAccountConfiguration(IntegrationTestBase.TestCredentials.AdditionalStorageAccounts[0].Name, IntegrationTestBase.TestCredentials.DefaultStorageAccount.Key));
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task NegativeTest_ExistingCluster_Using_PocoClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.NegativeTest_InvalidAsv_Using_PocoClient();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task NegativeTest_ExistingCluster_Using_PocoClient()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var cluster = base.GetRandomCluster();
            cluster.DnsName = TestCredentials.DnsName;
            await ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials).CreateContainer(cluster);
        }

        private async Task ValidateCreateClusterFailsWithError(CreateClusterRequest cluster)
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials);
            await client.CreateContainer(cluster);
            client.WaitForClusterCondition(
                cluster.DnsName,
                c => c != null,
                TimeSpan.FromMilliseconds(100));

            var result = await client.ListContainer(cluster.DnsName);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ResultError);

            await client.DeleteContainer(cluster.DnsName);
            client.WaitForClusterCondition(
                cluster.DnsName,
                c => c == null,
                TimeSpan.FromMilliseconds(100));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidMetastore_Using_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.NegativeTest_InvalidMetastore_Using_PocoClient();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_InvalidMetastore_Using_PocoClient()
        {
            var cluster = base.GetRandomCluster();
            cluster.OozieMetastore = new ComponentMetastore(TestCredentials.OozieStores[0].SqlServer,
                                                            TestCredentials.OozieStores[0].Database,
                                                            TestCredentials.OozieStores[0].Username,
                                                            TestCredentials.OozieStores[0].Password);
            cluster.HiveMetastore = new ComponentMetastore(TestCredentials.HiveStores[0].SqlServer,
                                                            TestCredentials.HiveStores[0].Database,
                                                            TestCredentials.HiveStores[0].Username,
                                                            "NOT-THE-REAL-PASSWORD");

            await ValidateCreateClusterFailsWithError(cluster);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_OnlyOneMetastore_Using_PocoClient()
        {
            var cluster = base.GetRandomCluster();
            cluster.OozieMetastore = new ComponentMetastore(TestCredentials.OozieStores[0].SqlServer,
                                                            TestCredentials.OozieStores[0].Database,
                                                            TestCredentials.OozieStores[0].Username,
                                                            TestCredentials.OozieStores[0].Password);

            await ValidateCreateClusterFailsWithError(cluster);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Production")]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("PocoClient")]
        public async Task NegativeTest_OnlyOneMetastore_Using_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await this.NegativeTest_OnlyOneMetastore_Using_PocoClient();
        }
    }
}