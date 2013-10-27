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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests;

    [TestClass]
    public class StartJobsCmdletIntegrationTests : StartJobsCmdletTestCaseBase
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
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("JobsIntegration")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewMapReduceJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewMapReduceJob_Then_Start_HDInsightJobsCmdlet();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("JobsIntegration")]
        [TestCategory("Start-AzureHDInsightJob")]
        public override void ICanCallThe_NewHiveJob_Then_Start_HDInsightJobsCmdlet()
        {
            base.ICanCallThe_NewHiveJob_Then_Start_HDInsightJobsCmdlet();
        }
    }
}
