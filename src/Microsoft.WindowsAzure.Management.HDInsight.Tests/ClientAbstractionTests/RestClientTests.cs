namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using System.Net;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.ConnectionCredentials;
    using Moq;

    [TestClass]
    public class RestClientTests : IntegrationTestBase
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
        [TestCategory("RestClient")]
        public void InternalValidation_HDInsightManagementRestClient_GetCloudServiceName()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var serviceName = HDInsightManagementRestClient.GetCloudServiceName(Guid.Empty, "hdInsight", "EastUS");
            Assert.AreEqual("hdInsightCK4TO7F6PZOJJ2FHBWOSHEUVEPIUV6UVI6JRGD4KHFM4POCJVSUA-EastUS", serviceName);

            var serviceName1 = HDInsightManagementRestClient.GetCloudServiceName(credentials.SubscriptionId, credentials.DeploymentNamespace, "EastUS");
            var serviceName2 = HDInsightManagementRestClient.GetCloudServiceName(credentials.SubscriptionId, credentials.DeploymentNamespace, "EastUS");
            Assert.AreEqual(serviceName1, serviceName2);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestClient")]
        public void InternalValidation_HDInsightRestClientException()
        {
            //TODO: Test serailize\deserialize

            // Validates that the exception is properly created and all the fields match what's expected
            try
            {
                throw new HDInsightRestClientException(HttpStatusCode.Ambiguous, "Hello world");
            }
            catch (HDInsightRestClientException exception)
            {
                Assert.AreEqual("Hello world", exception.RequestContent);
                Assert.AreEqual(HttpStatusCode.Ambiguous, exception.RequestStatusCode);
                Assert.AreEqual("Request failed with code:MultipleChoices\r\nContent:Hello world", exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("CheckIn")]
        [TestCategory("RestClient")]
        [TestCategory("Production")]
        public async Task ICanPerformA_ListCloudServices_Using_AzureProduction()
        {
            await this.ICanPerformA_ListCloudServices_Using_RestClientAbstraction();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestClient")]
        public async Task ICanPerformA_ListCloudServices_Using_RestClientAbstraction()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = ServiceLocator.Instance.Locate<IHDInsightManagementRestClientFactory>().Create(credentials);
            Assert.IsTrue(this.ContainsContainer(TestCredentials.DnsName, await client.ListCloudServices()));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestClient")]
        [Timeout(5*60*1000)] // ms
        public async Task ICanPerformA_CreateDeleteContainers_Using_RestClient()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            var client = new HDInsightManagementRestClient(credentials);
            var dnsName = base.GetRandomClusterName();
            var location = "East US";
            var subscriptionId = credentials.SubscriptionId;
            
            var createPayload = String.Format(CreateContainerGenericRequest, dnsName, location, subscriptionId, Guid.NewGuid());
            var xmlReader = new XmlTextReader(new StringReader(createPayload));
            var resource = new Resource()
            {
                IntrinsicSettings = new[] { new XmlDocument().ReadNode(xmlReader) }
            };

            Assert.IsTrue(!this.ContainsContainer(dnsName, await client.ListCloudServices()));

            await client.CreateContainer(dnsName, location, resource.SerializeToXml());
            while (!this.ContainsContainer(dnsName, await client.ListCloudServices()))
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            await client.DeleteContainer(dnsName, location);
            while (this.ContainsContainer(dnsName, await client.ListCloudServices()))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        
        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Manual")]
        [TestCategory("RestClient")]
        [Timeout(5 * 60 * 1000)] // ms
        public async Task ICanPerformA_CreateDeleteContainers_Using_RestClient_ManualEnvironment()
        {
            if (IntegrationTestBase.TestCredentials.AlternativeEnvironment == null)
                Assert.Inconclusive("Alternative Azure Endpoint wasn't set up");
            
            IConnectionCredentials credentials = new AlternativeEnvironmentConnectionCredentialsFactory().Create(
                IntegrationTestBase.TestCredentials.AlternativeEnvironment.SubscriptionId,
                IntegrationTestBase.GetValidCredentials().Certificate);
            
            var client = new HDInsightManagementRestClient(credentials);
            var dnsName = base.GetRandomClusterName();
            var location = "East US";
            var subscriptionId = credentials.SubscriptionId;

            var createPayload = String.Format(CreateContainerGenericRequest, dnsName, location, subscriptionId, Guid.NewGuid());
            var xmlReader = new XmlTextReader(new StringReader(createPayload));
            var resource = new Resource()
            {
                IntrinsicSettings = new[] { new XmlDocument().ReadNode(xmlReader) }
            };

            Assert.IsTrue(!this.ContainsContainer(dnsName, await client.ListCloudServices()));

            await client.CreateContainer(dnsName, location, resource.SerializeToXml());
            while (!this.ContainsContainer(dnsName, await client.ListCloudServices()))
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            await client.DeleteContainer(dnsName, location);
            while (this.ContainsContainer(dnsName, await client.ListCloudServices()))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        
        // dnsName, location, subscriptionId, Guid.NewGuid()
        private const string CreateContainerGenericRequest =
@"<ClusterContainer xmlns=""http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess.Context"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
    <ExtendedProperties i:nil=""true"" />
    <AzureStorageAccount i:nil=""true"" />
    <CNameMapping i:nil=""true"" />
    <ContainerError i:nil=""true"" />
    <ContainerState i:nil=""true"" />
    <Deployment i:nil=""true"" />
    <DeploymentAction>None</DeploymentAction>
    <DnsName>{0}</DnsName>
    <AzureStorageLocation>{1}</AzureStorageLocation>
    <SubscriptionId>{2}</SubscriptionId>
    <IncarnationID>{3}</IncarnationID>
</ClusterContainer>";

        private bool ContainsContainer(string dnsName, string payload)
        {
            string xmlContainer = string.Format("<Name>{0}</Name>", dnsName);
            return payload.Contains(xmlContainer);
        }
    }
}
