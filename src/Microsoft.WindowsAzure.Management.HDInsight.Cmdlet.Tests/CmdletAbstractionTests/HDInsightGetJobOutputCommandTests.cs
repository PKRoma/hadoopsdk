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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class HDInsightGetJobOutputCommandTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetJobErrorLogsForCompletedJob()
        {
            ClusterDetails cluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
            IGetAzureHDInsightJobCommand getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            AzureHDInsightJob jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            IGetAzureHDInsightJobOutputCommand getJobOutputCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.StandardError;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            Stream outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetJobOutputForCompletedJob()
        {
            ClusterDetails cluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
            IGetAzureHDInsightJobCommand getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            AzureHDInsightJob jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            IGetAzureHDInsightJobOutputCommand getJobOutputCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            Stream outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetTaskLogsForCompletedJob()
        {
            ClusterDetails cluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
            IGetAzureHDInsightJobCommand getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();


            AzureHDInsightJob jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));
            string logDirectoryPath = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
            IGetAzureHDInsightJobOutputCommand getJobOutputCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.TaskLogs;
            getJobOutputCommand.TaskLogsDirectory = logDirectoryPath;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            IEnumerable<string> logFiles = Directory.EnumerateFiles(logDirectoryPath);
            Assert.IsTrue(logFiles.Any());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Jobs")]
        [TestCategory("GetAzureHDInsightJobCommand")]
        public void CanGetTaskSummaryForCompletedJob()
        {
            ClusterDetails cluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
            IGetAzureHDInsightJobCommand getJobsCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobs();
            getJobsCommand.Credential = GetPSCredential(cluster.HttpUserName, cluster.HttpPassword);
            getJobsCommand.Cluster = cluster.ConnectionUrl;
            getJobsCommand.EndProcessing();

            AzureHDInsightJob jobWithStatusDirectory = getJobsCommand.Output.First(j => !string.IsNullOrEmpty(j.StatusDirectory));

            IGetAzureHDInsightJobOutputCommand getJobOutputCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            getJobOutputCommand.Subscription = cluster.SubscriptionId.ToString();
            getJobOutputCommand.Cluster = cluster.Name;
            getJobOutputCommand.OutputType = JobOutputType.TaskSummary;
            getJobOutputCommand.JobId = jobWithStatusDirectory.JobId;
            getJobOutputCommand.EndProcessing();

            Stream outputStream = getJobOutputCommand.Output.First();
            Assert.IsTrue(outputStream.Length > 0);
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        internal static JobList GetJobHistory(string clusterEndpoint)
        {
            string clusterGatewayUri = GatewayUriResolver.GetGatewayUri(clusterEndpoint).AbsoluteUri.ToUpperInvariant();
            var manager = ServiceLocator.Instance.Locate<IServiceLocationSimulationManager>();
            if (manager.MockingLevel == ServiceLocationMockingLevel.ApplyFullMocking)
            {
                if (AzureHDInsightJobSubmissionClientSimulatorFactory.jobSubmissionClients.ContainsKey(clusterGatewayUri))
                {
                    return AzureHDInsightJobSubmissionClientSimulatorFactory.jobSubmissionClients[clusterGatewayUri].ListJobs();
                }
            }

            return new JobList { ErrorCode = HttpStatusCode.NotFound.ToString() };
        }
    }
}
