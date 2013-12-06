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
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class HDInsightGetCommandTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Rdfe")]
        [TestCategory("GetAzureHDInsightClusterCommand")]
        public void ICanPerform_GetClusters_HDInsightGetCommand()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            IGetAzureHDInsightClusterCommand client = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
            client.Subscription = creds.SubscriptionId.ToString();
            client.Certificate = creds.Certificate;
            client.EndProcessing();
            IEnumerable<AzureHDInsightCluster> containers = from container in client.Output
                                                            where container.Name.Equals(TestCredentials.WellKnownCluster.DnsName)
                                                            select container;
            Assert.AreEqual(1, containers.Count());
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
