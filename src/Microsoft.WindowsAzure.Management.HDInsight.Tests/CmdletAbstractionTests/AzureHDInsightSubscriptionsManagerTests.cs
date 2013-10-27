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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdletAbstractionTests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    [TestClass]
    public class AzureHDInsightSubscriptionsManagerTests : IntegrationTestBase
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
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanLoadSubscriptionDataById()
        {
            var manager = new AzureHDInsightSubscriptionsManager();

            SubscriptionData subscriptionData;
            Assert.IsTrue(manager.TryGetSubscriptionData(new Guid("03453212-1457-4952-8b31-687db8e28af2"), out subscriptionData));
            Assert.AreNotEqual(Guid.Empty, subscriptionData.SubscriptionId);
            Assert.IsFalse(string.IsNullOrEmpty(subscriptionData.SubscriptionName));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanLoadSubscriptionDataByName()
        {
            var manager = new AzureHDInsightSubscriptionsManager();
            SubscriptionData subscriptionData;
            Assert.IsTrue(manager.TryGetSubscriptionData("Hadoop", out subscriptionData));
            Assert.AreNotEqual(Guid.Empty, subscriptionData.SubscriptionId);
            Assert.IsFalse(string.IsNullOrEmpty(subscriptionData.SubscriptionName));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CannotLoadSubscriptionDataIfSettingsFileIsAbsent()
        {
            using (var manager = new AzureHDInsightSubscriptionsFileManager())
            {
                try
                {
                    manager.LoadSubscriptionData("filedoesnotexist");
                }
                catch (NotSupportedException nsException)
                {
                    Assert.AreEqual(
                        "Please install Windows Azure Powershell Tools v 0.7.0 or higher from http://go.microsoft.com/?linkid=9811175&clcid=0x409.\r\n" +
                        "This version is required to enable auto detection of Certificates for Azure Subscriptions.",
                        nsException.Message);
                }
            }
        }
    }
}
