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

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Concreates;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;

    public class IntegrationTestBase
    {
        private static readonly IntegrationTestManager testManager = new IntegrationTestManager();
        internal static AzureTestCredentials TestCredentials;
        internal static Dictionary<string, string> testToClusterMap = new Dictionary<string, string>();

        protected static AzureTestCredentials GetCredentials(string name)
        {
            return testManager.GetCredentials(name);
        }

        protected static string ClusterPrefix;
        private static IConnectionCredentials validCredentials;
        private static IConnectionCredentials invalidSubscriptionId;
        private static IConnectionCredentials invalidCertificate;
        public TestContext TestContext { get; set; }
    
        protected void IndividualTestSetup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        protected void IndividualTestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        public static class TestRunNames
        {
            public const string Default = "default";
        }

        internal static void TestRunCleanup()
        {
            // First get the simulator clusters.
            var runManager = ServiceLocator.Instance.Locate<IIocServiceLocationTestRunManager>();
            runManager.MockingLevel = IocTestMockingLevel.ApplyTestRunMockingOnly;
            var factory = ServiceLocator.Instance.Locate<IClusterProvisioningClientFactory>();
            var creds = GetCredentials(TestRunNames.Default);
            var client = factory.Create(creds.SubscriptionId, new X509Certificate2(creds.Certificate));
            var clusters = client.ListClusters().ToList();
            var simClusters = clusters.Where(c => c.Name.StartsWith(ClusterPrefix, StringComparison.OrdinalIgnoreCase));

            foreach (var cluster in simClusters)
            {
                client.DeleteCluster(cluster.Name);
            }
            
            // Next get the live clusters.
            runManager.MockingLevel = IocTestMockingLevel.ApplyNoMocking;
            factory = ServiceLocator.Instance.Locate<IClusterProvisioningClientFactory>();
            client = factory.Create(creds.SubscriptionId, new X509Certificate2(creds.Certificate));
            clusters = client.ListClusters().ToList();
            var liveClusters = clusters.Where(c => c.Name.StartsWith(ClusterPrefix, StringComparison.OrdinalIgnoreCase));

            foreach (var cluster in liveClusters)
            {
                client.DeleteCluster(cluster.Name);
            }
        }

        internal static void TestRunSetup()
        {
            var testManager = new IntegrationTestManager();
            if (!testManager.RunAzureTests())
            {
                Assert.Inconclusive("Azure tests are not configured on this machine.");
            }
            IntegrationTestBase.TestCredentials = testManager.GetCredentials("default");
            if (IntegrationTestBase.TestCredentials == null)
            {
                Assert.Inconclusive("No entry was found in the credential config file for the specified test configuration.");
            }

            // Sets the certificate
            var defaultCertificate = new X509Certificate2(IntegrationTestBase.TestCredentials.Certificate); ;


            // Sets the test static properties
            IntegrationTestBase.ClusterPrefix = string.Format("CLITest-{0}", Environment.GetEnvironmentVariable("computername") ?? "unknown");

            // Sets the credential objects
            IntegrationTestBase.validCredentials = ServiceLocator.Instance
                                                .Locate<IConnectionCredentialsFactory>()
                                                .Create(TestCredentials.SubscriptionId, defaultCertificate);
            IntegrationTestBase.invalidSubscriptionId = ServiceLocator.Instance
                                                     .Locate<IConnectionCredentialsFactory>()
                                                     .Create(Guid.NewGuid(), defaultCertificate);
            IntegrationTestBase.invalidCertificate = ServiceLocator.Instance
                                                  .Locate<IConnectionCredentialsFactory>()
                                                  .Create(TestCredentials.SubscriptionId, new X509Certificate2(TestCredentials.InvalidCertificate));

            // Prepares the environment 
            IntegrationTestBase.CleanUpClusters();
        }

        private IRunspace runspace;

        internal IRunspace GetPowerShellRunspace()
        {
            if (this.runspace.IsNull())
            {
                var loc = typeof(GetAzureHDInsightClusterCmdlet).Assembly.Location;
                this.runspace = Help.SaveCreate(() => RunspaceAbstraction.Create());
                this.runspace.NewPipeline()
                             .AddCommand("Import-Module")
                             .WithParameter("Name", loc)
                             .Invoke();
            }
            return this.runspace;
        }

        public void ApplyIndividualTestMockingOnly()
        {
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationTestRunManager>();
            manager.MockingLevel = IocTestMockingLevel.ApplyIndividualTestMockingOnly;
        }

        public void ResetIndividualMocks()
        {
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();
            manager.Reset();
        }

        public void ApplyFullMocking()
        {
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationTestRunManager>();
            manager.MockingLevel = IocTestMockingLevel.ApplyFullMocking;
        }

        public void ApplyNoMocking()
        {
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationTestRunManager>();
            manager.MockingLevel = IocTestMockingLevel.ApplyNoMocking;
        }

        internal static IConnectionCredentials GetValidCredentials()
        {
            return validCredentials;
        }

        internal static IConnectionCredentials GetInvalidSubscriptionIdCredentails()
        {
            return invalidSubscriptionId;
        }

        internal static IConnectionCredentials GetInvalidCertificateCredentails()
        {
            return invalidCertificate;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Azure names must be lowercase.")]
        public string GetRandomClusterName()
        {
            // Random DNS name.
            var time = DateTime.UtcNow;
            var machineName = Environment.GetEnvironmentVariable("computername") ?? "unknown";
            var retval = string.Format("{0}-{1}{2}{3}-{4}",
                                       ClusterPrefix,
                                       time.Month.ToString("00"),
                                       time.Day.ToString("00"),
                                       time.Hour.ToString("00"),
                                       Guid.NewGuid().ToString("N")).ToLowerInvariant();
            testToClusterMap.Add(retval, System.Environment.StackTrace.ToString());
            return retval;
        }

        public string GetRandomValidPassword()
        {
            return Guid.NewGuid().ToString().ToUpperInvariant().Replace('A', 'a').Replace('B', 'b').Replace('C', 'c') + "forTest!";
        }

        public HDInsightClusterCreationDetails GetRandomCluster()
        {
            // Creates the cluster
            return new HDInsightClusterCreationDetails
            {
                Name = this.GetRandomClusterName(),
                UserName = TestCredentials.AzureUserName,
                Password = GetRandomValidPassword(),
                Location = "East US 2",
                DefaultStorageAccountName = TestCredentials.DefaultStorageAccount.Name,
                DefaultStorageAccountKey = TestCredentials.DefaultStorageAccount.Key,
                DefaultStorageContainer =  TestCredentials.DefaultStorageAccount.Container,
                ClusterSizeInNodes = 3
            };
        }

        internal static void CleanUpClusters()
        {
            var credentials = IntegrationTestBase.GetValidCredentials();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials))
            {
                var clusters = client.ListContainers();
                clusters.WaitForResult();
                foreach (var cluster in clusters.Result.Where(c => c.Name.StartsWith(ClusterPrefix)))
                {
                    client.DeleteContainer(cluster.Name, cluster.Location).WaitForResult();
                }
            }
        }

        internal static void DeleteClusters(IConnectionCredentials credentials, string location)
        {
            var client = new ClusterProvisioningClient(credentials.SubscriptionId, credentials.Certificate);
            var clusters = client.ListClusters().Where(cluster => cluster.Location == location).ToList();

            Parallel.ForEach(clusters, cluster => client.DeleteCluster(cluster.Name));
        }
    }
}