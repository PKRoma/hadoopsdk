namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;

    [TestClass]
    public static class TestSetupCleanup
    {
        private static List<Type> types = new List<Type>();

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // This is to ensure that all key assemblies are loaded before IOC registration is required.
            // This is only necessary for the test system as load order is correct for a production run.
            types.Add(typeof(NewAzureHDInsightClusterCmdlet));
            // Sets the simulator
            var runManager = ServiceLocator.Instance.Locate<IIocServiceLocationTestRunManager>();
            runManager.RegisterType<IHDInsightManagementRestClientFactory, HDInsightManagementRestSimulatorClientFactory>();
            runManager.RegisterType<ISubscriptionRegistrationClientFactory, SubscriptionRegistrationSimulatorClientFactory>();

            // Reads configurations
            IntegrationTestBase.TestRunSetup();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            IntegrationTestBase.TestRunCleanup();
        }
    }
}
