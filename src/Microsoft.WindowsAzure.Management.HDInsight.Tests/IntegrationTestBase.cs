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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Concreates;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;

    public class IntegrationTestBase
    {
        private readonly IntegrationTestManager testManager = new IntegrationTestManager();
        internal static AzureTestCredentials TestCredentials;
        protected AzureTestCredentials GetCredentials(string name)
        {
            return this.testManager.GetCredentials(name);
        }

        protected static string ClusterPrefix;
        private static IConnectionCredentials validCredentials;
        private static IConnectionCredentials invalidSubscriptionId;
        private static IConnectionCredentials invalidCertificate;
    
        internal static void TestSetup()
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

        internal IRunspace GetPowerShellRunspace()
        {
            var loc = typeof(GetAzureHDInsightClusterCmdlet).Assembly.Location;
            var runspace = Help.SaveCreate(() => RunspaceAbstraction.Create());
            runspace.NewPipeline()
                    .AddCommand("Import-Module")
                    .WithParameter("Name", loc)
                    .Invoke();
            return runspace;
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
            return string.Format("{0}-{1}{2}{3}-{4}",
                                 ClusterPrefix,
                                 time.Month.ToString("00"),
                                 time.Day.ToString("00"),
                                 time.Hour.ToString("00"),
                                 Guid.NewGuid().ToString("N")).ToLowerInvariant();
        }

        public string GetRandomValidPassword()
        {
            return Guid.NewGuid().ToString().ToUpperInvariant().Replace('A', 'a').Replace('B', 'b').Replace('C', 'c') + "forTest!";
        }

        public CreateClusterRequest GetRandomCluster()
        {
            // Creates the cluster
            return new CreateClusterRequest
            {
                DnsName = this.GetRandomClusterName(),
                ClusterUserName = TestCredentials.AzureUserName,
                ClusterUserPassword = GetRandomValidPassword(),
                Location = "East US",
                DefaultAsvAccountName = TestCredentials.DefaultStorageAccount.Name,
                DefaultAsvAccountKey = TestCredentials.DefaultStorageAccount.Key,
                DefaultAsvContainer =  TestCredentials.DefaultStorageAccount.Container,
                WorkerNodeCount = 3
            };
        }

        internal static void CleanUpClusters()
        {
            var credentials = IntegrationTestBase.GetValidCredentials();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightManagementPocoClientFactory>().Create(credentials))
            {
                var clusters = client.ListContainers();
                clusters.WaitForResult();
                foreach (var cluster in clusters.Result.Where(c => c.DnsName.StartsWith(ClusterPrefix)))
                {
                    client.DeleteContainer(cluster.DnsName, cluster.Location).WaitForResult();
                }
            }
        }
    }
}