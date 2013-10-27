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
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    [TestClass]
    public class AzureHDInsightClusterCommandBaseTests : IntegrationTestBase
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

        private X509Certificate2 TestCertificate
        {
            get { return new X509Certificate2(IntegrationTestBase.TestCredentials.Certificate); }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveSubscriptionId_ValidGuid()
        {
            var validGuid = Guid.NewGuid();
            var resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId(validGuid.ToString());
            Assert.AreEqual(validGuid, resolvedSubscriptionId, "User supplied subscriptionid should be honored");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveSubscriptionId_SubscriptionName()
        {
            var expectedSubscriptionId = "03453212-1457-4952-8b31-687db8e28af2";
            var resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId("Hadoop");
            Assert.AreEqual(expectedSubscriptionId, resolvedSubscriptionId.ToString(), "Cannot resolve subscription name.");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveCertificate_ValidCertificate()
        {
            var resolvedCertificate = AzureHDInsightCommandExtensions.ResolveCertificate(IntegrationTestBase.TestCredentials.SubscriptionId, new X509Certificate2(IntegrationTestBase.TestCredentials.Certificate));
            Assert.AreEqual(resolvedCertificate, this.TestCertificate);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolvCertificate_ValidSubscriptionId()
        {
            var resolvedCertificate = AzureHDInsightCommandExtensions.ResolveCertificate(IntegrationTestBase.TestCredentials.SubscriptionId, null);
            Assert.AreEqual(resolvedCertificate, this.TestCertificate);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolvCertificate_InvalidSubscriptionId()
        {
            AzureHDInsightCommandExtensions.ResolveCertificate(Guid.NewGuid(), null);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveSubscriptionId_InvalidSubscriptionName()
        {
            var invalidGuidString = "This is not a valid subscription";
            var resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId(invalidGuidString);
        }
    }
}
