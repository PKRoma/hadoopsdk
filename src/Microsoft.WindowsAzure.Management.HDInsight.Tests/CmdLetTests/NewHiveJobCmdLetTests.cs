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
    using System.Management.Automation;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    [TestClass]
    public class NewHiveJobCmdLetTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);
            }

        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithFileParameter()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                File = Constants.WabsProtocolSchemeName + "filepath.hql"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.File, HiveJobDefinition.File)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.File, HiveJobFromPowershell.File);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICannotCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithoutFileOrQueryParameter()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                File = Constants.WabsProtocolSchemeName + "filepath.hql"
            };

            try
            {
                using (var runspace = this.GetPowerShellRunspace())
                {
                    runspace.NewPipeline()
                         .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                         .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                         .Invoke();
                    Assert.Fail("test failed.");
                }
            }
            catch (CmdletInvocationException invokeException)
            {
                var psArgumentException = invokeException.GetBaseException() as PSArgumentException;
                Assert.IsNotNull(psArgumentException);
                Assert.AreEqual("Either File or Query should be specified for Hive jobs.", psArgumentException.Message);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithoutJobName()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                Query = "show tables"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithResources()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };
            HiveJobDefinition.Files.Add("pidata.txt");
            HiveJobDefinition.Files.Add("pidate2.txt");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .WithParameter(CmdletConstants.Files, HiveJobDefinition.Files)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);

                foreach (var file in HiveJobDefinition.Files)
                {
                    Assert.IsTrue(
                        HiveJobFromPowershell.Files.Any(arg => string.Equals(file, arg)),
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
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithArguments()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };
            HiveJobDefinition.Arguments.Add("arg 1");
            HiveJobDefinition.Arguments.Add("arg 2");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .WithParameter(CmdletConstants.HiveArgs, HiveJobDefinition.Arguments)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);

                foreach (var args in HiveJobDefinition.Arguments)
                {
                    Assert.IsTrue(
                        HiveJobFromPowershell.Arguments.Any(arg => string.Equals(args, arg)),
                        "Unable to find argument '{0}' in value returned from powershell",
                        args);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithParameters()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables"
            };

            HiveJobDefinition.Defines.Add("map.input.tasks", "1000");
            HiveJobDefinition.Defines.Add("map.input.reducers", "1000");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .WithParameter(CmdletConstants.Parameters, HiveJobDefinition.Defines)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);

                foreach (var parameter in HiveJobDefinition.Defines)
                {
                    Assert.IsTrue(
                        HiveJobFromPowershell.Defines.Any(arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
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
        [TestCategory("New-AzureHDInsightHiveJobDefinition")]
        [TestCategory("Defect")]
        public void ICanCallThe_New_HDInsightHiveJobDefinitionCmdlet_WithOutputStorageLocation()
        {
            var HiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "show tables jobDetails",
                Query = "show tables",
                StatusFolder = "/tablesList"
            };

            HiveJobDefinition.Defines.Add("map.input.tasks", "1000");
            HiveJobDefinition.Defines.Add("map.input.reducers", "1000");

            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.NewAzureHDInsightHiveJobDefinition)
                                      .WithParameter(CmdletConstants.JobName, HiveJobDefinition.JobName)
                                      .WithParameter(CmdletConstants.Query, HiveJobDefinition.Query)
                                      .WithParameter(CmdletConstants.Parameters, HiveJobDefinition.Defines)
                                      .WithParameter(CmdletConstants.StatusFolder, HiveJobDefinition.StatusFolder)
                                      .Invoke();
                Assert.AreEqual(1, results.Results.Count);
                var HiveJobFromPowershell = results.Results.ToEnumerable<AzureHDInsightHiveJobDefinition>().First();

                Assert.AreEqual(HiveJobDefinition.JobName, HiveJobFromPowershell.JobName);
                Assert.AreEqual(HiveJobDefinition.Query, HiveJobFromPowershell.Query);
                Assert.AreEqual(HiveJobDefinition.StatusFolder, HiveJobFromPowershell.StatusFolder);

                foreach (var parameter in HiveJobDefinition.Defines)
                {
                    Assert.IsTrue(
                        HiveJobFromPowershell.Defines.Any(arg => string.Equals(parameter.Key, arg.Key) && string.Equals(parameter.Value, arg.Value)),
                        "Unable to find parameter '{0}' in value returned from powershell",
                        parameter.Key);
                }
            }
        }
    }
}
