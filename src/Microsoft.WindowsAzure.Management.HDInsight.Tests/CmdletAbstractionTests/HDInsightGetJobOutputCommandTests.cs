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
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class HDInsightGetJobOutputCommandTests : IntegrationTestBase
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
        public void CanGetJobOutputForCompletedJob()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            var jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            var outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetJobErrorLogsForCompletedJob()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            var jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.StandardError;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            var outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetTaskSummaryForCompletedJob()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            var jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.TaskSummary;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            var outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetTaskLogsForCompletedJob()
        {
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            var jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));
            var logsDirectory = Directory.CreateDirectory(Guid.NewGuid().ToString());
            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.TaskLogs;
            getJobOutputCommand.TaskLogsDirectory = logsDirectory.Name;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            var logFiles = logsDirectory.EnumerateFiles();
            Assert.IsTrue(logFiles.Any());
        }

        internal static JobList GetJobHistory(string clusterEndpoint)
        {
            string clusterGatewayUri = GatewayUriResolver.GetGatewayUri(clusterEndpoint).AbsoluteUri.ToUpperInvariant();
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
