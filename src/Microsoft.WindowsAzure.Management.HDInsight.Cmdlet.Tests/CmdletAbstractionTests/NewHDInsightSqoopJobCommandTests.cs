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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests
{
    using System.Linq;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class NewHDInsightSqoopJobCommandTests : IntegrationTestBase
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
        [TestCategory("New-AzureHDInsightSqoopJobDefinition")]
        public void CanCreateNewSqoopDefinition()
        {
            var pigJobDefinition = new SqoopJobCreateParameters
            {
                Command = "load 'passwd' using SqoopStorage(':'); B = foreach A generate $0 as id;"
            };

            INewAzureHDInsightSqoopJobDefinitionCommand newSqoopJobDefinitionCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewSqoopDefinition();
            newSqoopJobDefinitionCommand.Command = pigJobDefinition.Command;
            newSqoopJobDefinitionCommand.EndProcessing();

            AzureHDInsightSqoopJobDefinition pigJobFromCommand = newSqoopJobDefinitionCommand.Output.ElementAt(0);

            Assert.AreEqual(pigJobDefinition.Command, pigJobFromCommand.Command);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightSqoopJobDefinition")]
        public void CanCreateNewSqoopDefinition_WithFile()
        {
            var pigJobDefinition = new SqoopJobCreateParameters { File = "my local file" };

            INewAzureHDInsightSqoopJobDefinitionCommand newSqoopJobDefinitionCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewSqoopDefinition();
            newSqoopJobDefinitionCommand.File = pigJobDefinition.File;
            newSqoopJobDefinitionCommand.EndProcessing();

            AzureHDInsightSqoopJobDefinition pigJobFromCommand = newSqoopJobDefinitionCommand.Output.ElementAt(0);

            Assert.AreEqual(pigJobDefinition.File, pigJobFromCommand.File);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        [TestCategory("Jobs")]
        [TestCategory("New-AzureHDInsightSqoopJobDefinition")]
        public void CanCreateNewSqoopDefinition_WithResources()
        {
            var pigJobDefinition = new SqoopJobCreateParameters
            {
                Command = "load 'passwd' using SqoopStorage(':'); B = foreach A generate $0 as id;"
            };
            pigJobDefinition.Files.Add("pidata.txt");
            pigJobDefinition.Files.Add("pidate2.txt");

            INewAzureHDInsightSqoopJobDefinitionCommand newSqoopJobDefinitionCommand =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewSqoopDefinition();
            newSqoopJobDefinitionCommand.Command = pigJobDefinition.Command;
            newSqoopJobDefinitionCommand.Files = pigJobDefinition.Files.ToArray();
            newSqoopJobDefinitionCommand.EndProcessing();

            AzureHDInsightSqoopJobDefinition pigJobFromCommand = newSqoopJobDefinitionCommand.Output.ElementAt(0);

            Assert.AreEqual(pigJobDefinition.Command, pigJobFromCommand.Command);

            foreach (string resource in pigJobDefinition.Files)
            {
                Assert.IsTrue(
                    pigJobFromCommand.Files.Any(arg => string.Equals(resource, arg)),
                    "Unable to find File '{0}' in value returned from command",
                    resource);
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
