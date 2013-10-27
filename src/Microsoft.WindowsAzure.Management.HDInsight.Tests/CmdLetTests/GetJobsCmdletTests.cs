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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Net.Http;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdletAbstractionTests;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator;

    [TestClass]
    public class GetJobsCmdletTests : IntegrationTestBase
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
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Get-AzureHDInsightJobs")]
        public void ICanCallThe_Get_HDInsightJobsCmdlet()
        {
            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                      .Invoke();
                var jobHistory = results.Results.ToEnumerable<AzureHDInsightJob>();

                var expectedJobHistory = HDInsightGetJobsCommandTests.GetJobHistory(testCluster.ConnectionUrl);
                Assert.AreEqual(expectedJobHistory.Jobs.Count, jobHistory.Count(), "Should have {0} jobs.", expectedJobHistory.Jobs.Count);
            }
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Get-AzureHDInsightJobs")]
        public void ICanCallThe_Get_HDInsightJobsCmdlet_WithDebug()
        {
            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                      .WithParameter(CmdletConstants.Debug, null)
                                      .Invoke();
                var jobHistory = results.Results.ToEnumerable<AzureHDInsightJob>();

                var expectedJobHistory = HDInsightGetJobsCommandTests.GetJobHistory(testCluster.ConnectionUrl);
                Assert.AreEqual(expectedJobHistory.Jobs.Count, jobHistory.Count(), "Should have {0} jobs.", expectedJobHistory.Jobs.Count);
                var expectedLogMessage = "Listing jobs";
                Assert.IsTrue(
                    logWriter.Buffer.Any(
                        message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Get-AzureHDInsightJobs")]
        public void ICanCallThe_Get_HDInsightJobsCmdlet_WithNonExistantJobId()
        {
            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                string jobId = Guid.NewGuid().ToString();
                var results = runspace.NewPipeline()
                                              .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                              .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                                              .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                              .WithParameter(CmdletConstants.JobId, jobId)
                                              .Invoke();
                Assert.AreEqual(results.Results.Count, 0);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Get-AzureHDInsightJobs")]
        public void ICanCallThe_Get_HDInsightJobsCmdletWithJobId()
        {
            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                      .Invoke();
                var jobDetail = results.Results.ElementAt(0).ImmediateBaseObject as AzureHDInsightJobBase;
                Assert.IsNotNull(jobDetail);

                var getJobDetailObj = GetJobWithID(runspace, jobDetail.JobId, testCluster);
                Assert.IsNotNull(getJobDetailObj);
                Assert.AreEqual(jobDetail.JobId, getJobDetailObj.JobId);
            }
        }

        internal static AzureHDInsightJob GetJobWithID(IRunspace runspace, string jobId, ClusterDetails cluster)
        {
            var getJobDetailResults =
                   runspace.NewPipeline()
                           .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                           .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                           .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                           .WithParameter(CmdletConstants.JobId, jobId)
                           .Invoke();

            return getJobDetailResults.Results.ToEnumerable<AzureHDInsightJob>().FirstOrDefault();
        }
    }
}
