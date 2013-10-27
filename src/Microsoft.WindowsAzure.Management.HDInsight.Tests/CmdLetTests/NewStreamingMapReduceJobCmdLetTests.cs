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
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    [TestClass]
    public class NewStreamingMapReduceJobCmdLetTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightStreamingJobDefinition")]
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet()
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

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
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
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
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
                                      .WithParameter(CmdletConstants.Input, streamingMapReduceJobDefinition.Input)
                                      .WithParameter(CmdletConstants.Output, streamingMapReduceJobDefinition.Output)
                                      .WithParameter(CmdletConstants.Mapper, streamingMapReduceJobDefinition.Mapper)
                                      .WithParameter(CmdletConstants.Reducer, streamingMapReduceJobDefinition.Reducer)
                                      .WithParameter(CmdletConstants.StatusFolder, streamingMapReduceJobDefinition.StatusFolder)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

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
            var streamingMapReduceJobDefinition = new StreamingMapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                Input = Constants.WabsProtocolSchemeName + "input",
                Output = Constants.WabsProtocolSchemeName + "input",
                Mapper = Constants.WabsProtocolSchemeName + "combiner",
                Reducer = Constants.WabsProtocolSchemeName + "combiner",
                StatusFolder = Constants.WabsProtocolSchemeName + "someotherlocation"
            };
            streamingMapReduceJobDefinition.Arguments.Add("16");
            streamingMapReduceJobDefinition.Arguments.Add("10000");

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
                                      .WithParameter(CmdletConstants.Arguments, streamingMapReduceJobDefinition.Arguments)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (var argument in streamingMapReduceJobDefinition.Arguments)
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
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithResources()
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
            streamingMapReduceJobDefinition.Files.Add("pidata.txt");
            streamingMapReduceJobDefinition.Files.Add("pidate2.txt");

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
                                      .WithParameter(CmdletConstants.Files, streamingMapReduceJobDefinition.Files)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (var file in streamingMapReduceJobDefinition.Files)
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
        public void ICanCallThe_New_HDInsightStreamingMapReduceJobDefinitionCmdlet_WithParameters()
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

            streamingMapReduceJobDefinition.Defines.Add("map.input.tasks", "1000");
            streamingMapReduceJobDefinition.Defines.Add("map.input.reducers", "1000");

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
                                      .WithParameter(CmdletConstants.Parameters, streamingMapReduceJobDefinition.Defines)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var streamingMapReduceJobFromPowershell =
                    results.Results.ToEnumerable<AzureHDInsightStreamingMapReduceJobDefinition>().First();

                AssertJobDefinitionsEqual(streamingMapReduceJobDefinition, streamingMapReduceJobFromPowershell);
                foreach (var parameter in streamingMapReduceJobDefinition.Defines)
                {
                    Assert.IsTrue(
                        streamingMapReduceJobFromPowershell.Defines.Any(arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
                        "Unable to find parameter '{0}' in value returned from powershell",
                        parameter.Key);
                }
            }
        }

        internal static void AssertJobDefinitionsEqual(
            StreamingMapReduceJobCreateParameters streamingMapReduceJobDefinition, AzureHDInsightStreamingMapReduceJobDefinition streamingMapReduceJobFromPowershell)
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
