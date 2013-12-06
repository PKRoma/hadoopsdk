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
    using System;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class SqoopJobDefinitionCmdletTests : IntegrationTestBase
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
        public void CanCreateSDKObjectFromPowershellObject()
        {
            var sqoopJobDefinition = new AzureHDInsightSqoopJobDefinition
            {
                Command = "Import into sqlserver",
                File = "http://myfileshare.txt",
                StatusFolder = Guid.NewGuid().ToString(),
            };

            sqoopJobDefinition.Arguments.Add("arg1");
            sqoopJobDefinition.Files.Add("file1.sqoop");
            SqoopJobCreateParameters sdkObject = sqoopJobDefinition.ToSqoopJobCreateParameters();

            Assert.AreEqual(sqoopJobDefinition.StatusFolder, sdkObject.StatusFolder);
            Assert.AreEqual(sqoopJobDefinition.File, sdkObject.File);
            Assert.AreEqual(sqoopJobDefinition.Command, sdkObject.Command);

            foreach (string file in sqoopJobDefinition.Files)
            {
                Assert.IsTrue(sdkObject.Files.Contains(file), file);
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
