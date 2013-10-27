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
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class StartAzureHDInsightJobCommandTests : IntegrationTestBase
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
        public void CanStartMapReduceJob()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };
            TestJobStart(mapReduceJobDefinition);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanAutoGenerateStatusDirectoryForMapReduceJob()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            var startedJob = TestJobStart(mapReduceJobDefinition);
            Assert.IsFalse(string.IsNullOrEmpty(startedJob.StatusDirectory));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void UserCanSupplyStatusDirectoryForMapReduceJob()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar",
                StatusFolder = "/myoutputfolder"
            };

            var startedJob = TestJobStart(mapReduceJobDefinition);
            Assert.AreEqual(startedJob.StatusDirectory, mapReduceJobDefinition.StatusFolder);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CannotStartCustomJobType()
        {
            try
            {
                var mapReduceJobDefinition = new HadoopCustomJobCreationDetails()
                {
                    CustomText = "pig text"
                };

                TestJobStart(mapReduceJobDefinition);
                Assert.Fail("An exception was expected");
            }
            catch (AggregateException aex)
            {
                var inner = aex.InnerExceptions.FirstOrDefault();
                Assert.IsNotNull(inner);
                Assert.IsInstanceOfType(inner, typeof(NotSupportedException));
                Assert.IsTrue(inner.Message.Contains("Cannot start jobDetails of type"));
                Assert.IsTrue(inner.Message.Contains("HadoopCustomJobCreationDetails"));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanStartHiveJob()
        {
            var hiveJobDefinition = new AzureHDInsightHiveJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                Query = "show tables;"
            };

            TestJobStart(hiveJobDefinition);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanStartSqoopJob()
        {
            var sqoopJobDefinition = new AzureHDInsightSqoopJobDefinition()
            {
                Command = "show tables;"
            };

            TestJobStart(sqoopJobDefinition);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewMapReduceJob_StartJob()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            var newMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewMapReduceDefinition();
            newMapReduceJobDefinitionCommand.JobName = mapReduceJobDefinition.JobName;
            newMapReduceJobDefinitionCommand.JarFile = mapReduceJobDefinition.JarFile;
            newMapReduceJobDefinitionCommand.ClassName = mapReduceJobDefinition.ClassName;
            newMapReduceJobDefinitionCommand.EndProcessing();

            var mapReduceJobFromCommand = (AzureHDInsightMapReduceJobDefinition)newMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobStart(mapReduceJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewMapReduceJob_WithoutJobName()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar",
                StatusFolder = "/myoutputfolder"
            };

            var startedJob = TestJobStart(mapReduceJobDefinition);
            Assert.AreEqual("pi", startedJob.Name);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewHiveJob_StartJob()
        {
            var hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            var newHiveJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewHiveDefinition();
            newHiveJobDefinitionCommand.JobName = hiveJobDefinition.JobName;
            newHiveJobDefinitionCommand.Query = hiveJobDefinition.Query;
            newHiveJobDefinitionCommand.EndProcessing();

            var hiveJobFromCommand = (AzureHDInsightHiveJobDefinition)newHiveJobDefinitionCommand.Output.ElementAt(0);
            TestJobStart(hiveJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewHiveJob_WithoutJobName_WithQuery()
        {
            var hiveJobDefinition = new AzureHDInsightHiveJobDefinition()
            {
                Query = "show tables"
            };

            var startedJob = TestJobStart(hiveJobDefinition);
            Assert.AreEqual(startedJob.Name, "Hive: show tables");
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewHiveJob_WithoutJobName_WithFile()
        {
            var hiveJobDefinition = new AzureHDInsightHiveJobDefinition()
            {
                File = Constants.WabsProtocolSchemeName + "container@hostname/Container1/myqueryfile.hql"
            };

            var startedJob = TestJobStart(hiveJobDefinition);
            Assert.AreEqual(startedJob.Name, "Hive: myqueryfile.hql");
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewPigJob_StartJob()
        {
            var pigJobDefinition = new PigJobCreateParameters()
            {
                Query = "load table from 'A'"
            };

            var newMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewPigJobDefinition();
            newMapReduceJobDefinitionCommand.Query = pigJobDefinition.Query;
            newMapReduceJobDefinitionCommand.EndProcessing();

            var pigJobFromCommand = (AzureHDInsightPigJobDefinition)newMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobStart(pigJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewPigJob_WithoutJobName_WithQuery()
        {
            var pigJobDefinition = new AzureHDInsightPigJobDefinition()
            {
                Query = "show tables"
            };

            var startedJob = TestJobStart(pigJobDefinition);
            Assert.AreEqual(string.Empty, startedJob.Name);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewPigJob_WithoutJobName_WithFile()
        {
            var pigJobDefinition = new AzureHDInsightPigJobDefinition()
            {
                File = Constants.WabsProtocolSchemeName + "container@hostname/Container1/myqueryfile.hql"
            };

            var startedJob = TestJobStart(pigJobDefinition);
            Assert.AreEqual(string.Empty, startedJob.Name);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewStreamingJob_StartJob()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                Input = Constants.WabsProtocolSchemeName + "container@hostname/input",
                Output = Constants.WabsProtocolSchemeName + "container@hostname/input",
                Mapper = Constants.WabsProtocolSchemeName + "container@hostname/combiner",
                Reducer = Constants.WabsProtocolSchemeName + "container@hostname/combiner",
                StatusFolder = Constants.WabsProtocolSchemeName + "container@hostname/someotherlocation"
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

            var streamingJobFromCommand = (AzureHDInsightStreamingMapReduceJobDefinition)newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            TestJobStart(streamingJobFromCommand);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void CanCreateNewStreamingMapReduceJob_WithoutJobName()
        {
            var streamingMapReduceJobDefinition = new AzureHDInsightStreamingMapReduceJobDefinition()
            {
                Input = Constants.WabsProtocolSchemeName + "container@hostname/input",
                Output = Constants.WabsProtocolSchemeName + "container@hostname/input",
                Mapper = Constants.WabsProtocolSchemeName + "container@hostname/mapper",
                Reducer = Constants.WabsProtocolSchemeName + "container@hostname/combiner",
                StatusFolder = Constants.WabsProtocolSchemeName + "container@hostname/someotherlocation"
            };

            var startedJob = TestJobStart(streamingMapReduceJobDefinition);
            Assert.AreEqual(startedJob.Name, "mapper");
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Defect")]
        public void CanCreateNewStreamingMapReduceJob_WithoutJobName_FilesName()
        {
            var streamingMapReduceJobDefinition = new AzureHDInsightStreamingMapReduceJobDefinition()
            {
                Input = "input",
                Output = "output",
                Mapper = "mapper.exe",
                Reducer = "combiner.exe",
                StatusFolder = "/someotherlocation"
            };

            var startedJob = TestJobStart(streamingMapReduceJobDefinition);
            Assert.AreEqual(startedJob.Name, "mapper.exe");
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Defect")]
        public void CanCreateNewStreamingMapReduceJob_WithoutJobName_FilesRelative()
        {
            var streamingMapReduceJobDefinition = new AzureHDInsightStreamingMapReduceJobDefinition()
            {
                Input = "input",
                Output = "output",
                Mapper = "/examples/mapper.exe",
                Reducer = "/examples/combiner.exe",
                StatusFolder = "/someotherlocation"
            };

            var startedJob = TestJobStart(streamingMapReduceJobDefinition);
            Assert.AreEqual("mapper.exe", startedJob.Name);
        }

        private static AzureHDInsightJob TestJobStart(AzureHDInsightJobDefinition mapReduceJobDefinition)
        {
            var testCluster = SyncClientScenarioTests.GetHttpAccessEnabledCluster();
            var startJobCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateStartJob();
            startJobCommand.Cluster = testCluster.ConnectionUrl;
            startJobCommand.Credential = IntegrationTestBase.GetPSCredential(testCluster.HttpUserName, testCluster.HttpPassword);
            startJobCommand.JobDefinition = mapReduceJobDefinition;
            startJobCommand.EndProcessing().Wait();
            var jobCreationResults = startJobCommand.Output.ElementAt(0);
            Assert.IsNotNull(jobCreationResults.JobId, "Should get a non-null jobDetails id");
            Assert.IsNotNull(jobCreationResults.StatusDirectory, "StatusDirectory should be set on jobDetails");
            return jobCreationResults;
        }
    }
}
