namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdletAbstractionTests
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet;

    [TestClass]
    public class HDInsightGetCommandTests : IntegrationTestBase
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
        [TestCategory("GetAzureHDInsightClusterCommand")]
        public void ICanPerform_GetClusters_HDInsightGetCommand()
        {
            var creds = GetValidCredentials();
            var client = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
            client.SubscriptionId = creds.SubscriptionId;
            client.Certificate = creds.Certificate;
            client.EndProcessing();
            var containers = from container in client.Output
                             where container.Name.Equals(TestCredentials.WellKnownCluster.DnsName)
                             select container;
            Assert.AreEqual(1, containers.Count());

            //var result = client.GetCluster("tsthdx00hdxcibld02");
            // Assert.IsNotNull(result);
            // Assert.AreEqual(result.Location, "East US");
            // Assert.AreEqual(result.UserName, "sa-po-svc");
        }
    }
}