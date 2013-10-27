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
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.Scenario;

    [TestClass]
    public class StarttJobsCmdletTests : StartJobsCmdletTestCaseBase
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
        public override void ICanCallThe_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_Start_HDInsightJobsCmdlet();
        }


        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public void ICanCallThe_Start_HDInsightJobsCmdlet_WithDebug()
        {
            var mapReduceJobDefinition = new AzureHDInsightMapReduceJobDefinition()
            {
                JobName = "pi estimation jobDetails",
                ClassName = "pi",
                JarFile = Constants.WabsProtocolSchemeName + "container@hostname/examples.jar"
            };

            using (var runspace = this.GetPowerShellRunspace())
            {
                var expectedLogMessage = string.Format(CultureInfo.InvariantCulture, "Starting jobDetails '{0}'.", mapReduceJobDefinition.JobName);
                RunJobInPowershell(runspace, mapReduceJobDefinition, SyncClientScenarioTests.GetHttpAccessEnabledCluster(), true, expectedLogMessage);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_Start_HDInsightJobsCmdlet_WithoutName()
        {
            base.ICanCallThe_Start_HDInsightJobsCmdlet_WithoutName();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewMapReduceJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewMapReduceJob_Then_Start_HDInsightJobsCmdlet();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewHiveJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewHiveJob_Then_Start_HDInsightJobsCmdlet();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewPigJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewPigJob_Then_Start_HDInsightJobsCmdlet();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewSqoopJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewSqoopJob_Then_Start_HDInsightJobsCmdlet();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewStreamingJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewStreamingJob_Then_Start_HDInsightJobsCmdlet();
        }
    }
}
