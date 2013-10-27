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
    public class NewMapReduceJobCmdLetTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithOutputStorageLocation()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar",
                StatusFolder = "/pilogs"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .WithParameter(CmdletConstants.StatusFolder, mapReduceJobDefinition.StatusFolder)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);
                Assert.AreEqual(mapReduceJobDefinition.StatusFolder, mapReduceJobFromPowershell.StatusFolder);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithoutJobName()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithArguments()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
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

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);

                foreach (var argument in mapReduceJobDefinition.Arguments)
                {
                    Assert.IsTrue(
                        mapReduceJobFromPowershell.Arguments.Any(arg => string.Equals(argument, arg)),
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
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithResources()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };
            mapReduceJobDefinition.Files.Add("pidata.txt");
            mapReduceJobDefinition.Files.Add("pidate2.txt");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .WithParameter(CmdletConstants.Files, mapReduceJobDefinition.Files)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);

                foreach (var file in mapReduceJobDefinition.Files)
                {
                    Assert.IsTrue(
                        mapReduceJobFromPowershell.Files.Any(arg => string.Equals(file, arg)),
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
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithLibJars()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };
            mapReduceJobDefinition.LibJars.Add("pidata.jar");
            mapReduceJobDefinition.LibJars.Add("pidate2.jar");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .WithParameter(CmdletConstants.LibJars, mapReduceJobDefinition.LibJars)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);

                foreach (var libjar in mapReduceJobDefinition.LibJars)
                {
                    Assert.IsTrue(
                        mapReduceJobFromPowershell.LibJars.Any(arg => string.Equals(libjar, arg)),
                        "Unable to find LibJar '{0}' in value returned from powershell",
                        libjar);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightMapReduceJobDefinition")]
        public void ICanCallThe_New_HDInsightMapReduceJobDefinitionCmdlet_WithParameters()
        {
            var mapReduceJobDefinition = new MapReduceJobCreateParameters()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            mapReduceJobDefinition.Defines.Add("map.input.tasks", "1000");
            mapReduceJobDefinition.Defines.Add("map.input.reducers", "1000");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightMapReduceJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, mapReduceJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.JarFile, mapReduceJobDefinition.JarFile)
                                      .WithParameter(CmdletConstants.ClassName, mapReduceJobDefinition.ClassName)
                                      .WithParameter(CmdletConstants.Parameters, mapReduceJobDefinition.Defines)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var mapReduceJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightMapReduceJobDefinition>().First();

                Assert.AreEqual(mapReduceJobDefinition.JobName, mapReduceJobFromPowershell.JobName);
                Assert.AreEqual(mapReduceJobDefinition.ClassName, mapReduceJobFromPowershell.ClassName);
                Assert.AreEqual(mapReduceJobDefinition.JarFile, mapReduceJobFromPowershell.JarFile);

                foreach (var parameter in mapReduceJobDefinition.Defines)
                {
                    Assert.IsTrue(
                        mapReduceJobFromPowershell.Defines.Any(arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
                        "Unable to find parameter '{0}' in value returned from powershell",
                        parameter.Key);
                }
            }
        }
    }
}
