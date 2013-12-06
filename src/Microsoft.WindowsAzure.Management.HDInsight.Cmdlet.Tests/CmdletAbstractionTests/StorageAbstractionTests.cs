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
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class StorageAbstractionTests : IntegrationTestBase
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
        [TestCategory("Nightly")]
        public void CanWriteHiveQueryFile()
        {
            this.ApplyIndividualTestMockingOnly();
            var wellKnownStorageAccount = IntegrationTestBase.GetWellKnownStorageAccounts().First();
            var wabsStorageClient = new AzureHDInsightStorageHandler(wellKnownStorageAccount);
            string hiveQueryFilePath = string.Format(CultureInfo.InvariantCulture,
                "http://{0}/{1}/user/{2}/{3}.hql",
                wellKnownStorageAccount.Name,
                wellKnownStorageAccount.Container,
                IntegrationTestBase.TestCredentials.HadoopUserName,
                Guid.NewGuid().ToString("N"));
            var testFilePath = new Uri(hiveQueryFilePath, UriKind.RelativeOrAbsolute);
            var bytes = Encoding.UTF8.GetBytes("Select * from hivesampletable where name like '%bat%'");
            using (var stream = new MemoryStream(bytes, 0, bytes.Length))
            {
                wabsStorageClient.UploadFile(testFilePath, stream);
            }
        }
    }
}
