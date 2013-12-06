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
    using System.Linq;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class NewStreamingMapReduceJobCmdLetTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters
            {
                JobName = "pi estimation jobDetails",
                Input = TestConstants.WabsProtocolSchemeName + "input",
                Output = TestConstants.WabsProtocolSchemeName + "input",
                Mapper = TestConstants.WabsProtocolSchemeName + "combiner",
                Reducer = TestConstants.WabsProtocolSchemeName + "combiner",
                StatusFolder = TestConstants.WabsProtocolSchemeName + "someotherlocation"
            };

            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                            .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithArguments()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters
            {
                JobName = "pi estimation jobDetails",
                Input = TestConstants.WabsProtocolSchemeName + "input",
                Output = TestConstants.WabsProtocolSchemeName + "input",
                Mapper = TestConstants.WabsProtocolSchemeName + "combiner",
                Reducer = TestConstants.WabsProtocolSchemeName + "combiner",
                StatusFolder = TestConstants.WabsProtocolSchemeName + "someotherlocation"
            };
            streamingMapReduceJobDefinition.Arguments.Add("16");
            streamingMapReduceJobDefinition.Arguments.Add("10000");

            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                            .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                            .WithParameter(CmdletConstants.Arguments, streamingMapReduceJobDefinition.Arguments)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (string argument in streamingMapReduceJobDefinition.Arguments)
                {
                    Assert.IsTrue(
                        streamingMapReduceJobFromPowershell.Arguments.Any(arg => string.Equals(argument, arg)),
                        "Unable to find argument '{0}' in value returned from powershell",
                        argument);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithParameters()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters
            {
                JobName = "pi estimation jobDetails",
                Input = TestConstants.WabsProtocolSchemeName + "input",
                Output = TestConstants.WabsProtocolSchemeName + "input",
                Mapper = TestConstants.WabsProtocolSchemeName + "combiner",
                Reducer = TestConstants.WabsProtocolSchemeName + "combiner",
                StatusFolder = TestConstants.WabsProtocolSchemeName + "someotherlocation"
            };

            streamingMapReduceJobDefinition.Defines.Add("map.input.tasks", "1000");
            streamingMapReduceJobDefinition.Defines.Add("map.input.reducers", "1000");

            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                            .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                            .WithParameter(CmdletConstants.Parameters, streamingMapReduceJobDefinition.Defines)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (var parameter in streamingMapReduceJobDefinition.Defines)
                {
                    Assert.IsTrue(
                        streamingMapReduceJobFromPowershell.Defines.Any(
                            arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
                        "Unable to find parameter '{0}' in value returned from powershell",
                        parameter.Key);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithResources()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters
            {
                JobName = "pi estimation jobDetails",
                Input = TestConstants.WabsProtocolSchemeName + "input",
                Output = TestConstants.WabsProtocolSchemeName + "input",
                Mapper = TestConstants.WabsProtocolSchemeName + "combiner",
                Reducer = TestConstants.WabsProtocolSchemeName + "combiner",
                StatusFolder = TestConstants.WabsProtocolSchemeName + "someotherlocation"
            };
            streamingMapReduceJobDefinition.Files.Add("pidata.txt");
            streamingMapReduceJobDefinition.Files.Add("pidate2.txt");

            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.JobName, streamingMapReduceJobDefinition.JobName)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                            .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                            .WithParameter(CmdletConstants.Files, streamingMapReduceJobDefinition.Files)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (string file in streamingMapReduceJobDefinition.Files)
                {
                    Assert.IsTrue(
                        streamingMapReduceJobFromPowershell.Files.Any(arg => string.Equals(file, arg)),
                        "Unable to find File '{0}' in value returned from powershell",
                        file);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithoutJobName()
        {
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters
            {
                Input = TestConstants.WabsProtocolSchemeName + "input",
                Output = TestConstants.WabsProtocolSchemeName + "input",
                Mapper = TestConstants.WabsProtocolSchemeName + "combiner",
                Reducer = TestConstants.WabsProtocolSchemeName + "combiner",
                StatusFolder = TestConstants.WabsProtocolSchemeName + "someotherlocation"
            };

            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.NewAzureHDInsightStreamingMapReduceJobDefinition)
                            .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                            .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                            .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                            .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                            .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                            .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        internal static void AssertJobDefinitionsEqual(
            StreamingMapReduceJobCreateParameters streamingMapReduceJobDefinition,
            AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell)
        {
            Assert.AreEqual(streamingMapReduceJobDefinition.JobName, streamingMapReduceJobFromPowershell.JobName);
            Assert.AreEqual(streamingMapReduceJobDefinition.Input, streamingMapReduceJobFromPowershell.Input);
            Assert.AreEqual(streamingMapReduceJobDefinition.Output, streamingMapReduceJobFromPowershell.Output);
            Assert.AreEqual(streamingMapReduceJobDefinition.Mapper, streamingMapReduceJobFromPowershell.Mapper);
            Assert.AreEqual(streamingMapReduceJobDefinition.Reducer, streamingMapReduceJobFromPowershell.Reducer);
            Assert.AreEqual(streamingMapReduceJobDefinition.StatusFolder, streamingMapReduceJobFromPowershell.StatusFolder);
        }
    }
}
