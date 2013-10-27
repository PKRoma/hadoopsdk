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
    using System.Linq;
    using System.Net.Http;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class StopAzureHDInsightJobCommandTests : IntegrationTestBase
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
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanStopMapReduceJob()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            TestJobLifecycle(mapReduceJobDefinition);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CannotStopNonExistingJob()
        {
            var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            TestJobStop(testCluster, Guid.NewGuid().ToString());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanStopHiveJob()
        {
            var hiveJobDefinition = new AzureHDInsightHiveJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                Query = "show tables;"
            };

            TestJobLifecycle(hiveJobDefinition);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewMapReduceJob_StartJob_StopJob()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation job",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            var newMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewMapReduceDefinition();
            newMapReduceJobDefinitionCommand.JobName = mapReduceJobDefinition.JobName;
            newMapReduceJobDefinitionCommand.JarFile = mapReduceJobDefinition.JarFile;
            newMapReduceJobDefinitionCommand.ClassName = mapReduceJobDefinition.ClassName;
            newMapReduceJobDefinitionCommand.EndProcessing();

            var mapReduceJobFromCommand = newMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobLifecycle(mapReduceJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewHiveJob_StartJob_StopJob()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            var newMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewHiveDefinition();
            newMapReduceJobDefinitionCommand.JobName = hiveJobDefinition.JobName;
            newMapReduceJobDefinitionCommand.Query = hiveJobDefinition.Query;
            newMapReduceJobDefinitionCommand.EndProcessing();

            var hiveJobFromCommand = newMapReduceJobDefinitionCommand.Output.ElementAt(0);
            TestJobLifecycle(hiveJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewPigJob_StartJob_StopJob()
        {
            var pigJobDefinition = new PigJobCreateParameters()
            {
                Query = "load table from 'A'"
            };

            var newMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewPigJobDefinition();
            newMapReduceJobDefinitionCommand.Query = pigJobDefinition.Query;
            newMapReduceJobDefinitionCommand.EndProcessing();

            var pigJobFromCommand = newMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobLifecycle(pigJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewStreamingJob_StartJob_StopJob()
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

            var newStreamingMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewStreamingMapReduceDefinition();
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.InputPath = streamingMapReduceJobDefinition.Input;
            newStreamingMapReduceJobDefinitionCommand.OutputPath = streamingMapReduceJobDefinition.Output;
            newStreamingMapReduceJobDefinitionCommand.Mapper = streamingMapReduceJobDefinition.Mapper;
            newStreamingMapReduceJobDefinitionCommand.Reducer = streamingMapReduceJobDefinition.Reducer;
            newStreamingMapReduceJobDefinitionCommand.StatusFolder = streamingMapReduceJobDefinition.StatusFolder;
            newStreamingMapReduceJobDefinitionCommand.EndProcessing();

            var streamingJobFromCommand = newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobLifecycle(streamingJobFromCommand);
        }

        private static AzureHDInsightJob TestJobStart(AzureHDInsightJobDefinition mapReduceJobDefinition)
        {
            var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            return TestJobStart(mapReduceJobDefinition, testCluster);
        }

        private static void TestJobLifecycle(AzureHDInsightJobDefinition mapReduceJobDefinition)
        {
            var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var startedJob = TestJobStart(mapReduceJobDefinition, testCluster);
            TestJobStop(testCluster, startedJob.JobId);
        }

        private static AzureHDInsightJob TestJobStart(AzureHDInsightJobDefinition mapReduceJobDefinition, ClusterDetails testCluster)
        {
            var startJobCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateStartJob();
            startJobCommand.Cluster = testCluster.ConnectionUrl;
            startJobCommand.Credential = IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword);
            startJobCommand.JobDefinition = mapReduceJobDefinition;
            startJobCommand.EndProcessing();
            var jobCreationResults = startJobCommand.Output.ElementAt(0);
            Assert.IsNotNull(jobCreationResults.JobId, "Should get a non-null jobDetails id");
            Assert.IsNotNull(jobCreationResults.StatusDirectory, "StatusDirectory should be set on jobDetails");
            return jobCreationResults;
        }

        private static AzureHDInsightJob TestJobStop(ClusterDetails testCluster, string jobId)
        {
            var stopJobCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateStopJob();
            stopJobCommand.Cluster = testCluster.ConnectionUrl;
            stopJobCommand.Credential = IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword);
            stopJobCommand.JobId = jobId;
            stopJobCommand.EndProcessing();

            if (stopJobCommand.Output.Count != 0)
            {
                var jobCancellationResults = stopJobCommand.Output.ElementAt(0);
                Assert.IsNotNull(jobCancellationResults.JobId, "Should get a non-null jobDetails id");
                Assert.IsNotNull(jobCancellationResults.StatusDirectory, "StatusDirectory should be set on jobDetails");
                return jobCancellationResults;
            }
            return null;
        }
    }
}
