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

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests
{
    using System;
    using ConnectionContext;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BasicUnitTests : IntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void TheTestSystemIsWorking()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void ICanGetCertificateInformation()
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            Assert.IsNotNull(credentials.Endpoint);
            Assert.AreNotEqual(Guid.Empty,
                               credentials.SubscriptionId);
            Assert.IsNotNull(credentials.Certificate);
        }
    }
}