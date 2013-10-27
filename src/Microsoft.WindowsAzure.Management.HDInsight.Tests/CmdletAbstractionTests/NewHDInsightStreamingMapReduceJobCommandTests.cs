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
    using System.Collections;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests;

    [TestClass]
    public class NewHDInsightStreamingMapReduceJobCommandTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightStreamingMapReduceJobDefinition")]
        public void CanCreateNewStreamingMapReduceDefinition()
        {
            var streamingMapReduceJobDefinition = GetStreamingMapReduceJob();

            var newStreamingMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewStreamingMapReduceDefinition();
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.InputPath = streamingMapReduceJobDefinition.Input;
            newStreamingMapReduceJobDefinitionCommand.OutputPath = streamingMapReduceJobDefinition.Output;
            newStreamingMapReduceJobDefinitionCommand.Mapper = streamingMapReduceJobDefinition.Mapper;
            newStreamingMapReduceJobDefinitionCommand.Reducer = streamingMapReduceJobDefinition.Reducer;
            newStreamingMapReduceJobDefinitionCommand.StatusFolder = streamingMapReduceJobDefinition.StatusFolder;
            newStreamingMapReduceJobDefinitionCommand.EndProcessing();

            var streamingMapReduceJobFromCommand = newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            NewStreamingMapReduceJobCmdLetTests.AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromCommand);

        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingMapReduceJobDefinition")]
        public void CanCreateNewStreamingMapReduceDefinition_WithArguments()
        {
            var streamingMapReduceJobDefinition = this.GetStreamingMapReduceJob();
            streamingMapReduceJobDefinition.Arguments.Add("16");
            streamingMapReduceJobDefinition.Arguments.Add("10000");

            var newStreamingMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewStreamingMapReduceDefinition();
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.InputPath = streamingMapReduceJobDefinition.Input;
            newStreamingMapReduceJobDefinitionCommand.OutputPath = streamingMapReduceJobDefinition.Output;
            newStreamingMapReduceJobDefinitionCommand.Mapper = streamingMapReduceJobDefinition.Mapper;
            newStreamingMapReduceJobDefinitionCommand.Reducer = streamingMapReduceJobDefinition.Reducer;
            newStreamingMapReduceJobDefinitionCommand.StatusFolder = streamingMapReduceJobDefinition.StatusFolder;
            newStreamingMapReduceJobDefinitionCommand.Arguments = streamingMapReduceJobDefinition.Arguments.ToArray();
            newStreamingMapReduceJobDefinitionCommand.EndProcessing();

            var streamingMapReduceJobFromCommand = newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            NewStreamingMapReduceJobCmdLetTests.AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromCommand);

            foreach (var argument in streamingMapReduceJobDefinition.Arguments)
            {
                Assert.IsTrue(
                    streamingMapReduceJobFromCommand.Arguments.Any(arg => string.Equals(argument, arg)),
                    "Unable to find argument '{0}' in value returned from command",
                    argument);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingMapReduceJobDefinition")]
        public void CanCreateNewStreamingMapReduceDefinition_WithResources()
        {
            var streamingMapReduceJobDefinition = this.GetStreamingMapReduceJob();
            streamingMapReduceJobDefinition.Files.Add("pidata.txt");
            streamingMapReduceJobDefinition.Files.Add("pidate2.txt");

            var newStreamingMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewStreamingMapReduceDefinition();
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.InputPath = streamingMapReduceJobDefinition.Input;
            newStreamingMapReduceJobDefinitionCommand.OutputPath = streamingMapReduceJobDefinition.Output;
            newStreamingMapReduceJobDefinitionCommand.Mapper = streamingMapReduceJobDefinition.Mapper;
            newStreamingMapReduceJobDefinitionCommand.Reducer = streamingMapReduceJobDefinition.Reducer;
            newStreamingMapReduceJobDefinitionCommand.StatusFolder = streamingMapReduceJobDefinition.StatusFolder;
            newStreamingMapReduceJobDefinitionCommand.Files = streamingMapReduceJobDefinition.Files.ToArray();
            newStreamingMapReduceJobDefinitionCommand.EndProcessing();

            var streamingMapReduceJobFromCommand = newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            NewStreamingMapReduceJobCmdLetTests.AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromCommand);

            foreach (var resource in streamingMapReduceJobDefinition.Files)
            {
                Assert.IsTrue(
                    streamingMapReduceJobFromCommand.Files.Any(arg => string.Equals(resource, arg)),
                    "Unable to find File '{0}' in value returned from command",
                    resource);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingMapReduceJobDefinition")]
        public void CanCreateNewStreamingMapReduceDefinition_WithParameters()
        {
            var streamingMapReduceJobDefinition = this.GetStreamingMapReduceJob();

            streamingMapReduceJobDefinition.Defines.Add("map.input.tasks", "1000");
            streamingMapReduceJobDefinition.Defines.Add("map.input.reducers", "1000");

            var newStreamingMapReduceJobDefinitionCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewStreamingMapReduceDefinition();
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.JobName = streamingMapReduceJobDefinition.JobName;
            newStreamingMapReduceJobDefinitionCommand.InputPath = streamingMapReduceJobDefinition.Input;
            newStreamingMapReduceJobDefinitionCommand.OutputPath = streamingMapReduceJobDefinition.Output;
            newStreamingMapReduceJobDefinitionCommand.Mapper = streamingMapReduceJobDefinition.Mapper;
            newStreamingMapReduceJobDefinitionCommand.Reducer = streamingMapReduceJobDefinition.Reducer;
            newStreamingMapReduceJobDefinitionCommand.StatusFolder = streamingMapReduceJobDefinition.StatusFolder;
            newStreamingMapReduceJobDefinitionCommand.Defines = new Hashtable();
            foreach (var define in streamingMapReduceJobDefinition.Defines)
            {
                newStreamingMapReduceJobDefinitionCommand.Defines.Add(define.Key, define.Value);
            }
            newStreamingMapReduceJobDefinitionCommand.EndProcessing();

            var streamingMapReduceJobFromCommand = newStreamingMapReduceJobDefinitionCommand.Output.ElementAt(0);

            NewStreamingMapReduceJobCmdLetTests.AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromCommand);

            foreach (var parameter in streamingMapReduceJobDefinition.Defines)
            {
                Assert.IsTrue(
                    streamingMapReduceJobFromCommand.Defines.Any(arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
                    "Unable to find parameter '{0}' in value returned from command",
                    parameter.Key);
            }
        }

        private StreamingMapReduceJobCreateParameters GetStreamingMapReduceJob()
        {
            return new StreamingMapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                Input = Constants.WabsProtocolSchemeName + "input",
                Output = Constants.WabsProtocolSchemeName + "input",
                Mapper = Constants.WabsProtocolSchemeName + "combiner",
                Reducer = Constants.WabsProtocolSchemeName + "combiner",
                StatusFolder = Constants.WabsProtocolSchemeName + "someotherlocation"
            };
        }
    }
}
