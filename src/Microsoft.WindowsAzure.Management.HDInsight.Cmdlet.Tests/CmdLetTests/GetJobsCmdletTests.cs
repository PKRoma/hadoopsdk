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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class GetJobsCmdletTests : IntegrationTestBase
    {
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
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ClusterDetails testCluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
                IPipelineResult results =
                    runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                            .WithParameter(CmdletConstants.Credential, GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                      .Invoke();
                IEnumerable<AzureHDInsightJob> jobHistory = results.Results.ToEnumerable<AzureHDInsightJob>();

                JobList expectedJobHistory = HDInsightGetJobsCommandTests.GetJobHistory(testCluster.ConnectionUrl);
                Assert.AreEqual(expectedJobHistory.Jobs.Count, jobHistory.Count(), "Should have {0} jobs.", expectedJobHistory.Jobs.Count);
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
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ClusterDetails testCluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                            .WithParameter(CmdletConstants.Credential, GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                            .Invoke();
                var jobDetail = results.Results.ElementAt(0).ImmediateBaseObject as AzureHDInsightJobBase;
                Assert.IsNotNull(jobDetail);

                AzureHDInsightJob getJobDetailObj = GetJobWithID(runspace, jobDetail.JobId, testCluster);
                Assert.IsNotNull(getJobDetailObj);
                Assert.AreEqual(jobDetail.JobId, getJobDetailObj.JobId);
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
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ClusterDetails testCluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                IPipelineResult results =
                    runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                            .WithParameter(CmdletConstants.Credential, GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                      .WithParameter(CmdletConstants.Debug, null)
                                      .Invoke();
                IEnumerable<AzureHDInsightJob> jobHistory = results.Results.ToEnumerable<AzureHDInsightJob>();

                JobList expectedJobHistory = HDInsightGetJobsCommandTests.GetJobHistory(testCluster.ConnectionUrl);
                Assert.AreEqual(expectedJobHistory.Jobs.Count, jobHistory.Count(), "Should have {0} jobs.", expectedJobHistory.Jobs.Count);
                string expectedLogMessage = "Listing jobs";
                Assert.IsTrue(logWriter.Buffer.Any(message => message.Contains(expectedLogMessage)));
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
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                ClusterDetails testCluster = CmdletScenariosTestCaseBase.GetHttpAccessEnabledCluster();
                string jobId = Guid.NewGuid().ToString();
                IPipelineResult results =
                    runspace.NewPipeline()
                                              .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                              .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                            .WithParameter(CmdletConstants.Credential, GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword))
                                              .WithParameter(CmdletConstants.Id, jobId)
                                              .Invoke();
                Assert.AreEqual(results.Results.Count, 0);
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            }

        internal static AzureHDInsightJob GetJobWithID(IRunspace runspace, string jobId, ClusterDetails cluster)
        {
            IPipelineResult getJobDetailResults =
                   runspace.NewPipeline()
                           .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                           .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                        .WithParameter(CmdletConstants.Credential, GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                           .WithParameter(CmdletConstants.Id, jobId)
                           .Invoke();

            return getJobDetailResults.Results.ToEnumerable<AzureHDInsightJob>().FirstOrDefault();
        }
    }
}
