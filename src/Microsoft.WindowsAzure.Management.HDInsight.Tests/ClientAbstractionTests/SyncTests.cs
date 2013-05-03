namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    [TestClass]
    public class SyncTests : IntegrationTestBase
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

        private class ThrowMockRestClientFactory : IHDInsightManagementRestClientFactory
        {
            private class ThrowMockRestClient : IHDInsightManagementRestClient
            {
                public void Dispose()
                {
                }

                public Task<string> ListCloudServices()
                {
                    throw new NotImplementedException("Mock Throw Exception");
                }

                public Task CreateContainer(string dnsName, string location, string clusterPayload)
                {
                    throw new NotImplementedException("Mock Throw Exception");
                }

                public Task DeleteContainer(string dnsName, string location)
                {
                    throw new NotImplementedException("Mock Throw Exception");
                }
            }

            public IHDInsightManagementRestClient Create(IConnectionCredentials creds)
            {
                return new ThrowMockRestClient();
            }
        }

        [TestMethod]
        [TestCategory("Defect")]
        [TestCategory(TestRunMode.CheckIn)]
        public void WhenIGetAnExceptionFromTheSyncLayer_ItIsNotAnAggregateException()
        {
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();
            manager.Override<IHDInsightManagementRestClientFactory>(new ThrowMockRestClientFactory());
            var factory = ServiceLocator.Instance.Locate<IHDInsightSyncClientFactory>();
            try
            {
                var syncClient = factory.Create(Guid.Empty, null);
                syncClient.ListContainers();
                Assert.Fail("This test expected an exception but failed to receive the exception.");
            }
            catch (Exception ex)
            {
                Assert.IsNotInstanceOfType(ex, typeof(AggregateException));
                Assert.IsInstanceOfType(ex, typeof(NotImplementedException));
                Assert.AreEqual("Mock Throw Exception", ex.Message);
            }
        }
    }
}
