namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    [TestClass]
    public class SubscriptionRegistrationTests : IntegrationTestBase
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
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        [TestCategory("Scenario")]
        public async Task ICanPerformA_PositiveSubscriptionValidation_Using_SubscriptionRegistrationAbstraction_AgainstAzure() // Always goes against azure to quickly validate end2end
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new SubscriptionRegistrationClient(credentials);
            Assert.IsTrue(await client.ValidateSubscriptionLocation("East US"));
            Assert.IsFalse(await client.ValidateSubscriptionLocation("No Where"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICanPerformA_PositiveSubscriptionValidation_Using_SubscriptionRegistrationAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            Assert.IsTrue(await client.ValidateSubscriptionLocation("East US"));
            Assert.IsFalse(await client.ValidateSubscriptionLocation("No Where"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICanPerformA_RepeatedSubscriptionRegistration_Using_SubscriptionRegistrationAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            await client.RegisterSubscription();
            await client.RegisterSubscription();
            await client.RegisterSubscriptionLocation("North Europe");
            await client.RegisterSubscriptionLocation("North Europe");
            Assert.IsTrue(await client.ValidateSubscriptionLocation("North Europe"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICanPerformA_RepeatedSubscriptionRegistration_Using_SubscriptionRegistrationAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICanPerformA_RepeatedSubscriptionRegistration_Using_SubscriptionRegistrationAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICannotPerformA_RepeatedUnregistration_Using_SubscriptionRegistrationAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            string location = "North Europe";
            DeleteClusters(credentials, location);

            // Need to delete clusters, otherwise unregister will no-op
            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            try
            {
                await client.UnregisterSubscriptionLocation("North Europe");
                await client.UnregisterSubscriptionLocation("North Europe");
                Assert.Fail("Expected exception.");
            }
            catch (HDInsightRestClientException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.RequestStatusCode);
                // Error looks like The cloud service with name [namespace] was not found.
                Assert.IsTrue(e.RequestContent.Contains("The cloud service with name"));
                Assert.IsTrue(e.RequestContent.Contains("was not found."));
            }

            Assert.IsFalse(await client.ValidateSubscriptionLocation(location));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICannotPerformA_RepeatedUnregistration_Using_SubscriptionRegistrationAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICannotPerformA_RepeatedUnregistration_Using_SubscriptionRegistrationAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICannotPerformA_UnregisterIfClustersExist_Using_SubscriptionRegistrationAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
           
            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            try
            {
                await client.UnregisterSubscriptionLocation("East US");
                Assert.Fail("Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual(e.Message, "Cannot unregister a subscription location if it contains clusters");
            }

            Assert.IsTrue(await client.ValidateSubscriptionLocation("East US"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICannotPerformA_UnregisterIfClustersExist_Using_SubscriptionRegistrationAbstraction_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICannotPerformA_UnregisterIfClustersExist_Using_SubscriptionRegistrationAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("SubcriptionRegistrationClient")]
        public async Task ICanPerformA_UnregisterSubscription_Using_SubscriptionRegistrationAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            DeleteClusters(credentials, "North Europe");

            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            await client.RegisterSubscriptionLocation("North Europe");
            await client.UnregisterSubscriptionLocation("North Europe");
        }
        
        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("RestClient")]
        [TestCategory("SubcriptionRegistrationClient")]
        [TestCategory("CheckIn")]
        public async Task ICannotPerformA_CreateContainersOnUnregisterdSubscription_Using_RestClient()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();

            // Unregisters location
            var location = "North Europe";
            DeleteClusters(credentials, location);
            var client = ServiceLocator.Instance.Locate<ISubscriptionRegistrationClientFactory>().Create(credentials);
            if (await client.ValidateSubscriptionLocation(location))
            {
                await client.UnregisterSubscriptionLocation(location);
            }
            Assert.IsFalse(await client.ValidateSubscriptionLocation(location));

            try
            {
                // Creates the cluster
                using (var restClient = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(credentials))
                {
                    var cluster = base.GetRandomCluster();
                    string payload = PayloadConverter.SerializeClusterCreateRequest(cluster, credentials.SubscriptionId);
                    await restClient.CreateContainer(cluster.Name, location, payload);
                }

                Assert.Fail("Expected exception.");
            }
            catch (HDInsightRestClientException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.RequestStatusCode);
                // Error looks like The cloud service with name [namespace] was not found.
                Assert.IsTrue(e.RequestContent.Contains("The cloud service with name"));
                Assert.IsTrue(e.RequestContent.Contains("was not found."));
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("RestClient")]
        [TestCategory("SubcriptionRegistrationClient")]
        [TestCategory("Scenario")]
        public async Task ICannotPerformA_CreateContainersOnUnregisterdSubscription_Using_RestClient_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            await ICannotPerformA_CreateContainersOnUnregisterdSubscription_Using_RestClient();
        }


        private void DeleteClusters(IConnectionCredentials credentials, string location)
        {
            var managementeClient = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            var clusters = managementeClient.ListClustersAsync().WaitForResult().Where(cluster => cluster.Location == location).ToList();

            Parallel.ForEach(clusters, cluster => managementeClient.DeleteClusterAsync(cluster.Name).WaitForResult());
        }
    }
}