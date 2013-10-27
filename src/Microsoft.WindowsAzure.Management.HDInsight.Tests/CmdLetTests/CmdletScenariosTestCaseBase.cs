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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class CmdletScenariosTestCaseBase : IntegrationTestBase
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
        public virtual void NewHiveJob_StartJob_GetJob()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                            .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 10)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJobOutput)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<string>());
                Assert.IsTrue(results.Results.ToEnumerable<string>().Any(str => str == "hivesampletable"));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void NewMapReduceJob_StartJob_GetJob()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = "/example/hadoop-examples.jar"
            };

            mapReduceJobDefinition.Arguments.Add("16");
            mapReduceJobDefinition.Arguments.Add("10000");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .WithParameter(CmdletConstants.Arguments, mapReduceJobDefinition.Arguments)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();
                RunJobAndGetWithId(runspace, mapReduceJobFromPowershell);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void NewStreamingMapReduceJob_StartJob_GetJob()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
                JobName = "environment variable jobDetails",
                Mapper = "environmentvariables.exe",
                Input = "/example/apps/environmentvariables.exe",
                Output = Guid.NewGuid().ToString()
            };

            var files = new List<string>() { streamingMapReduceJobDefinition.Mapper };
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                                      .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                                      .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                                      .WithParameter(CmdletConstants.Files, files)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();
                RunJobAndGetWithId(runspace, streamingMapReduceJobFromPowershell);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void NewPigJob_StartJob_GetJob()
        {
            var pigJobDefinition = new PigJobCreateParameters()
            {
                Query = "load 'passwd' using PigStorage(':'); B = foreach A generate $0 as id;"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightPigJobDefinition)
                                      .WithParameter(CmdletConstants.Query, pigJobDefinition.Query)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var pigJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightPigJobDefinition>().First();
                var pigJobfromHistory = RunJobAndGetWithId(runspace, pigJobFromPowershell);
                Assert.AreEqual(pigJobfromHistory.Query, pigJobDefinition.Query, "Failed to retrieve query for executed pig jobDetails");
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void NewStreamingJob_StartJob_GetJob()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                Input = Constants.WabsProtocolSchemeName + "input",
                Output = Constants.WabsProtocolSchemeName + "input",
                Mapper = Constants.WabsProtocolSchemeName + "combiner",
                Reducer = Constants.WabsProtocolSchemeName + "combiner",
                StatusFolder = Constants.WabsProtocolSchemeName + "someotherlocation"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                                      .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                                      .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                                      .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                                      .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                RunJobAndGetWithId(runspace, streamingMapReduceJobFromPowershell);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void PipeliningHiveJobExecution()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                            .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 10)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJobOutput)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<string>());
                Assert.IsTrue(results.Results.ToEnumerable<string>().Any(str => str == "hivesampletable"));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void PipeliningHiveJobExecution_FlowCluster()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                            .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 10)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJobOutput)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<string>());
                Assert.IsTrue(results.Results.ToEnumerable<string>().Any(str => str == "hivesampletable"));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void PipeliningHiveJobExecution_Start_GetJob()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                            .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<AzureHDInsightJob>());
                Assert.IsTrue(results.Results.ToEnumerable<AzureHDInsightJob>().Any(job => job.Name == "show tables"));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void PipeliningMapReduceJobExecution()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation job",
                ClassName = "pi",
                JarFile = "/example/hadoop-examples.jar"
            };

            mapReduceJobDefinition.Arguments.Add("16");
            mapReduceJobDefinition.Arguments.Add("10000");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                            .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                            .WithParameter(CmdletConstants.Arguments, mapReduceJobDefinition.Arguments)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 10)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJobOutput)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<string>());
                Assert.IsTrue(results.Results.ToEnumerable<string>().Any(str => str == "3.142"));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Jobs")]
        public virtual void PipeliningStreamingMapReduceJobExecution()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
                JobName = "pi estimation job",
                Mapper = "environmentvariables.exe",
                Input = "/example/apps/environmentvariables.exe",
                Output = Guid.NewGuid().ToString()
            };

            var files = new List<string>() { streamingMapReduceJobDefinition.Mapper };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Files, files)
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.WaitTimeoutInSeconds, 10)
                            .AddCommand(CmdletConstants.GetAzureHDInsightJobOutput)
                            .WithParameter(CmdletConstants.Subscription, testCluster.SubscriptionId)
                            .WithParameter(CmdletConstants.Cluster, testCluster.Name)
                            .Invoke();

                Assert.IsNotNull(results.Results.ToEnumerable<string>());
                Assert.IsTrue(results.Results.ToEnumerable<string>().Any(str => str == "3.142"));
            }
        }

        private static AzureHDInsightJob RunJobAndGetWithId<TJobType>(IRunspace runspace, TJobType jobDefinition) where TJobType : AzureHDInsightJobDefinition
        {
            var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var testClusterCredentials = IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword);
            var jobWithIdResults =
                runspace.NewPipeline()
                        .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                        .WithParameter(CmdletConstants.Cluster, testCluster.ConnectionUrl)
                        .WithParameter(CmdletConstants.Credential, testClusterCredentials)
                        .WithParameter(CmdletConstants.JobDefinition, jobDefinition)
                        .AddCommand(CmdletConstants.WaitAzureHDInsightJob)
                        .WithParameter(CmdletConstants.Credential, testClusterCredentials)
                        .Invoke();

            var jobWithId = jobWithIdResults.Results.ToEnumerable<AzureHDInsightJob>().First();
            Assert.AreEqual(jobWithId.State, JobStatusCode.Completed.ToString(), "jobDetails failed.");
            return jobWithId;
        }
    }
}
