using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System.Linq;
    using System.Management.Automation.Runspaces;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    [TestClass]
    public class GetCommandCmdletTests : IntegrationTestBase
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
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(TestCredentials.WellKnownCluster.DnsName, ((AzureHDInsightCluster)results.Results.ElementAt(0).ImmediateBaseObject).Name);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void ICanCallThe_Get_ClusterHDInsightClusterCmdlet_WithADnsName()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletHardCodes.GetAzureHDInsightCluster)
                                      .WithParameter(CmdletHardCodes.SubscriptionId, creds.SubscriptionId)
                                      .WithParameter(CmdletHardCodes.Certificate, creds.Certificate)
                                      .WithParameter(CmdletHardCodes.Name, TestCredentials.WellKnownCluster.DnsName)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual(TestCredentials.WellKnownCluster.DnsName, ((AzureHDInsightCluster)results.Results.ElementAt(0).ImmediateBaseObject).Name);
            }
        }
    }
}
