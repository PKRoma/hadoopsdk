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
    using System;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class AzureHDInsightClusterCommandBaseTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveCertificate_InvalidSubscriptionId()
        {
            AzureHDInsightCommandExtensions.ResolveCertificate(Guid.NewGuid(), null);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [Ignore]
        public void ResolveCertificate_ValidSubscriptionId()
        {
            X509Certificate2 resolvedCertificate = AzureHDInsightCommandExtensions.ResolveCertificate(TestCredentials.SubscriptionId, null);
            Assert.AreEqual(resolvedCertificate, this.TestCertificate);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveCertificate_SubscriptionId_NullCertificate()
        {
            var subscriptionId = new Guid("96B8BEAF-6498-4F5E-853B-522474A64BF8");
            try
            {
                AzureHDInsightCommandExtensions.ResolveCertificate(subscriptionId, null);
                Assert.Fail();
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to retrieve Certificate for the subscription '{0}'.\r\n" +
                        "Please register subcription certificate as described here: http://go.microsoft.com/fwlink/?LinkID=325564&clcid=0x409",
                        subscriptionId),
                    argumentException.Message);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveCertificate_SubscriptionName_NullCertificate()
        {
            Guid subscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId("Null_Management_Certificate");
            try
            {
                AzureHDInsightCommandExtensions.ResolveCertificate(subscriptionId, null);
                Assert.Fail();
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to retrieve Certificate for the subscription '{0}'.\r\n" +
                        "Please register subcription certificate as described here: http://go.microsoft.com/fwlink/?LinkID=325564&clcid=0x409",
                        subscriptionId),
                    argumentException.Message);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveCertificate_ValidCertificate()
        {
            X509Certificate2 resolvedCertificate = AzureHDInsightCommandExtensions.ResolveCertificate(
                TestCredentials.SubscriptionId, new X509Certificate2(TestCredentials.Certificate));
            Assert.AreEqual(resolvedCertificate, this.TestCertificate);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentException))]
        public void ResolveSubscriptionId_InvalidSubscriptionName()
        {
            string invalidGuidString = "This is not a valid subscription";
            Guid resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId(invalidGuidString);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveSubscriptionId_SubscriptionName()
        {
            string expectedSubscriptionId = "03453212-1457-4952-8b31-687db8e28af2";
            Guid resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId("Hadoop");
            Assert.AreEqual(expectedSubscriptionId, resolvedSubscriptionId.ToString(), "Cannot resolve subscription name.");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        public void ResolveSubscriptionId_ValidGuid()
        {
            Guid validGuid = Guid.NewGuid();
            Guid resolvedSubscriptionId = AzureHDInsightCommandExtensions.ResolveSubscriptionId(validGuid.ToString());
            Assert.AreEqual(validGuid, resolvedSubscriptionId, "User supplied subscriptionid should be honored");
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private X509Certificate2 TestCertificate
        {
            get { return new X509Certificate2(TestCredentials.Certificate); }
        }
    }
}
