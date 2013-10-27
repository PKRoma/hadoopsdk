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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class WaitCmdletTest : IntegrationTestBase
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
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void WaitForJob()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            IHadoopClientExtensions.GetPollingInterval = () => 0;
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                      .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                      .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual("Completed", results.Results.ToEnumerable<AzureHDInsightJob>().First().State);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void WaitForInvalidJobDoesNotThrow()
        {
            var jobDetails = new JobDetails()
            {
                JobId = Guid.NewGuid().ToString()
            };
            var invalidJob = new AzureHDInsightJob(jobDetails, IntegrationTestBase.TestCredentials.WellKnownCluster.DnsName);

            IHadoopClientExtensions.GetPollingInterval = () => 0;
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var runspace = this.GetPowerShellRunspace())
            {
                runspace.NewPipeline()
                .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                .WithParameter(CmdletConstants.Job, invalidJob)
                .Invoke();
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void WaitForJobWithTimeout()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            IHadoopClientExtensions.GetPollingInterval = () => 0;
            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                      .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                      .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                      .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 0.01)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                Assert.AreEqual("Completed", results.Results.ToEnumerable<AzureHDInsightJob>().First().State);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public void WaitForJobs()
        {
            IHadoopClientExtensions.GetPollingInterval = () => 0;
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            var cluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                      .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                      .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                      .Invoke();

                int startingCount = results.Results.Count;

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                  .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                  .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                  .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                                  .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                  .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                  .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                  .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                  .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                  .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                                  .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                  .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                  .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                  .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                  .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                  .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                                  .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                  .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                  .Invoke();

                results = runspace.NewPipeline()
                                  .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                                  .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                                  .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                  .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                                  .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                                  .Invoke();
                Assert.AreEqual(startingCount + 3, results.Results.Count);
                foreach (var entity in results.Results.ToEnumerable<AzureHDInsightJob>())
                {
                    Assert.IsTrue(entity.State == "Completed" || entity.State == "Canceled" || entity.State == "Failed");
                }
            }
        }
    }
}
