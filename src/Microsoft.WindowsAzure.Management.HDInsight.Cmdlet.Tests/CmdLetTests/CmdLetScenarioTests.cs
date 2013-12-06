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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CmdLetScenarioTestCaseTests : CmdletScenariosTestCaseBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Nightly")]
        [TestCategory("Jobs")]
        public void NewHiveJob_StartJob_GetJob_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            base.NewHiveJob_StartJob_GetJob();
        }

        [TestMethod]
        [TestCategory("Nightly")]
        [TestCategory("Jobs")]
        public void NewMapReduceJob_StartJob_GetJob_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            base.NewMapReduceJob_StartJob_GetJob();
        }

        [TestMethod]
        [TestCategory("Nightly")]
        [TestCategory("Jobs")]
        public void NewPigJob_StartJob_GetJob_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            base.NewPigJob_StartJob_GetJob();
        }

        [TestMethod]
        [TestCategory("Nightly")]
        [TestCategory("Jobs")]
        public void NewStreamingJob_StartJob_GetJob_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            base.NewStreamingJob_StartJob_GetJob();
        }

        [TestMethod]
        [TestCategory("Nightly")]
        [TestCategory("Jobs")]
        public void NewStreamingMapReduceJob_StartJob_GetJob_AgainstAzure()
        {
            this.ApplyIndividualTestMockingOnly();
            base.NewStreamingMapReduceJob_StartJob_GetJob();
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
