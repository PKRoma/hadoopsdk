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
    using System.Linq;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    public class StartJobsCmdletTestCaseBase : IntegrationTestBase
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

        public virtual void ICanCallThe_Start_HDInsightJobsCmdlet()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                RunJobInPowershell(runspace, mapReduceJobDefinition);
            }
        }

        public virtual void ICanCallThe_Start_HDInsightJobsCmdlet_WithoutName()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                RunJobInPowershell(runspace, mapReduceJobDefinition);
            }
        }

        public virtual void ICanCallThe_NewMapReduceJob_Then_Start_HDInsightJobsCmdlet()
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

                RunJobInPowershell(runspace, mapReduceJobFromPowershell);
            }
        }

        public virtual void ICanCallThe_NewHiveJob_Then_Start_HDInsightJobsCmdlet()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                         .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                         .WithParameter(CmdletConstants.JobName, hiveJobDefinition.JobName)
                                         .WithParameter(CmdletConstants.Query, hiveJobDefinition.Query)
                                         .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var hiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();
                var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
                var jobCreationDetails = RunJobInPowershell(runspace, hiveJobFromPowershell, testCluster);
                var jobHistoryResult = GetJobsCmdletTests.GetJobWithID(runspace, jobCreationDetails.JobId, testCluster);
            }
        }

        public virtual void ICanCallThe_NewPigJob_Then_Start_HDInsightJobsCmdlet()
        {
            var pigJobDefinition = new PigJobCreateParameters()
            {
                Query = "load table from 'A'"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                         .AddCommand(CmdletConstants.NewAzureHDInsightPigJobDefinition)
                                         .WithParameter(CmdletConstants.Query, pigJobDefinition.Query)
                                         .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var pigJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightPigJobDefinition>().First();

                RunJobInPowershell(runspace, pigJobFromPowershell);
            }
        }

        public virtual void ICanCallThe_NewSqoopJob_Then_Start_HDInsightJobsCmdlet()
        {
            var sqoopJobDefinition = new SqoopJobCreateParameters()
            {
                Command = "load table from 'A'"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                         .AddCommand(CmdletConstants.NewAzureHDInsightSqoopJobDefinition)
                                         .WithParameter(CmdletConstants.Command, sqoopJobDefinition.Command)
                                         .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var sqoopJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightSqoopJobDefinition>().First();

                RunJobInPowershell(runspace, sqoopJobFromPowershell);
            }
        }

        public virtual void ICanCallThe_NewStreamingJob_Then_Start_HDInsightJobsCmdlet()
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
                var streamingJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                RunJobInPowershell(runspace, streamingJobFromPowershell);
            }
        }

        internal static AzureHDInsightJob RunJobInPowershell(IRunspace runspace, AzureHDInsightJobDefinition mapReduceJobDefinition)
        {
            return RunJobInPowershell(runspace, mapReduceJobDefinition, SyncClientScenarioTests.GetHttpAccessEnabledCluster());
        }

        internal static AzureHDInsightJob RunJobInPowershell(IRunspace runspace, AzureHDInsightJobDefinition mapReduceJobDefinition, ClusterDetails cluster)
        {
            var results =
                 runspace.NewPipeline()
                         .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                         .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                         .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                         .WithParameter(CmdletConstants.JobDefinition, mapReduceJobDefinition)
                         .Invoke();
            Assert.AreEqual(1, results.Results.Count);
            var jobCreationCmdletResults = results.Results.ToEnumerable<AzureHDInsightJob>();
            var jobCreationResults = jobCreationCmdletResults.First();
            Assert.IsNotNull(jobCreationResults.JobId, "Should get a non-null jobDetails id");

            return jobCreationResults;
        }


        internal static AzureHDInsightJob RunJobInPowershell(IRunspace runspace, AzureHDInsightJobDefinition mapReduceJobDefinition, ClusterDetails cluster, bool debug, string expectedLogMessage)
        {
            IPipelineResult result = null;
            if (debug)
            {
                var logWriter = new PowershellLogWriter();
                BufferingLogWriterFactory.Instance = logWriter;
                result =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                            .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                            .WithParameter(
                                CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                            .WithParameter(CmdletConstants.JobDefinition, mapReduceJobDefinition)
                            .WithParameter(CmdletConstants.Debug, null)
                            .Invoke();

                Assert.IsTrue(logWriter.Buffer.Any(message => message.Contains(expectedLogMessage)));
                BufferingLogWriterFactory.Reset();
            }
            else
            {
                result = runspace.NewPipeline()
                             .AddCommand(CmdletConstants.StartAzureHDInsightJob)
                             .WithParameter(CmdletConstants.Cluster, cluster.ConnectionUrl)
                             .WithParameter(CmdletConstants.Credential, IntegrationTestBase.GetPSCredential(cluster.HttpUserName, cluster.HttpPassword))
                             .WithParameter(CmdletConstants.JobDefinition, mapReduceJobDefinition)
                             .Invoke();
            }
            Assert.AreEqual(1, result.Results.Count);
            var jobCreationCmdletResults = result.Results.ToEnumerable<AzureHDInsightJob>();
            var jobCreationResults = jobCreationCmdletResults.First();
            Assert.IsNotNull(jobCreationResults.JobId, "Should get a non-null jobDetails id");

            return jobCreationResults;
        }
    }
}
