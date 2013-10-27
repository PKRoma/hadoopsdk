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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdletAbstractionTests
{
    using System;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.WebHCatRest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Management.Automation.Runspaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using System.Management.Automation;
    using System.Security;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;

    [TestClass]
    public class HDInsightGetJobsCommandTests : IntegrationTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanListJobsForAValidCluster()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            var history = GetJobHistory(getJobsCommand.Cluster);

            Assert.AreEqual(history.Jobs.Count, getJobsCommand.Output.Count, "Should have {0} jobs.", history.Jobs.Count);
            foreach (var job in getJobsCommand.Output)
            {
                Assert.IsFalse(string.IsNullOrEmpty(job.PercentComplete));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanListJobsForAValidClusterWithJobId()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var psCredentials = GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = psCredentials;
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            if (!getJobsCommand.Output.Any())
                return;

            var jobDetail = getJobsCommand.Output.First();

            var getJobWithIdCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobWithIdCommand.Credential = psCredentials;
            getJobWithIdCommand.Cluster = cluster.ConnectionUrl;
            getJobWithIdCommand.JobId = jobDetail.JobId;
            getJobWithIdCommand.EndProcessing();

            Assert.AreEqual(1, getJobWithIdCommand.Output.Count, "Should have only one jobDetails when called with jobId.");
            Assert.AreEqual(jobDetail.JobId, getJobsCommand.Output.First().JobId, "Should get jobDetails with the same jobId as the one requested.");
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetHDInsightHadoopClientWithSubscriptionCertificateCreds()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var cmd = new GetAzureHDInsightJobCommand())
            {
                var creds = IntegrationTestBase.GetValidCredentials();
                cmd.Cluster = cluster.Name;
                cmd.Subscription = creds.SubscriptionId.ToString();
                cmd.Certificate = creds.Certificate;

                var client = cmd.GetClient(cmd.Cluster);
                Assert.IsNotNull(client);
                Assert.IsInstanceOfType(client, typeof(HDInsightHadoopClient));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetRemoteHadoopClientWithSubscriptionCertificateCreds()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var cmd = new GetAzureHDInsightJobCommand())
            {
                var creds = IntegrationTestBase.GetValidCredentials();
                cmd.Cluster = cluster.Name;
                cmd.Subscription = creds.SubscriptionId.ToString();
                cmd.Certificate = creds.Certificate;

                var client = cmd.GetClient(cmd.Cluster);
                Assert.IsNotNull(client);
                Assert.IsInstanceOfType(client, typeof(HDInsightHadoopClient));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetClientWithoutCredentialsorSubscriptionCertificateThrows()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var cmd = new GetAzureHDInsightJobCommand())
            {
                cmd.Cluster = cluster.Name;

                cmd.GetClient(cmd.Cluster);
            }
        }

        internal static JobList GetJobHistory(string clusterEndpoint)
        {
            string clusterGatewayUri = JobSubmission.GatewayUriResolver.GetGatewayUri(clusterEndpoint).AbsoluteUri.ToUpperInvariant();
            var manager = ServiceLocator.Instance.Locate<IServiceLocationSimulationManager>();
            if (manager.MockingLevel == ServiceLocationMockingLevel.ApplyFullMocking)
            {
                if (HadoopJobSubmissionPocoSimulatorClientFactory.pocoSimulators.ContainsKey(clusterGatewayUri))
                {
                    var history =
                        HadoopJobSubmissionPocoSimulatorClientFactory.pocoSimulators[clusterGatewayUri].ListJobs();

                    return history.WaitForResult();
                }
            }

            return new JobList()
            {
                ErrorCode = HttpStatusCode.NotFound.ToString()
            };
        }
    }
}