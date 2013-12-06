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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class HDInsightGetPropertiesCommandTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void HDInsightDefaultStorageContainerToStringIsAccountName()
        {
            string accountName = "storageaccountname.blob.core.windows.net";
            var storageAccount = new AzureHDInsightDefaultStorageAccount
            {
                StorageAccountKey = Guid.NewGuid().ToString(),
                StorageAccountName = accountName,
                StorageContainerName = "default"
            };

            Assert.AreEqual(accountName, storageAccount.ToString());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void HDInsightStorageContainerToStringIsAccountName()
        {
            string accountName = "storageaccountname.blob.core.windows.net";
            var storageAccount = new AzureHDInsightStorageAccount { StorageAccountKey = Guid.NewGuid().ToString(), StorageAccountName = accountName };

            Assert.AreEqual(accountName, storageAccount.ToString());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void HDInsightVersionToStringIsVersionNumber()
        {
            string version = "1.4.0.0.LargeAMD64SKU";
            var hdInsightVersion = new HDInsightVersion { Version = version, VersionStatus = VersionStatus.Obsolete };
            Assert.AreEqual(version, hdInsightVersion.ToString());
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("GetAzureHDInsightPropertiesCommand")]
        public void ICanPerform_GetProperties_HDInsightGetCommand()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            IGetAzureHDInsightPropertiesCommand client = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetProperties();
            client.Subscription = creds.SubscriptionId.ToString();
            client.Certificate = creds.Certificate;
            client.EndProcessing();

            ValidateCapabilities(client.Output);
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        internal static void ValidateCapabilities(IEnumerable<AzureHDInsightCapabilities> capabilities)
        {
            var versions = new Collection<HDInsightVersion>();
            versions.Add(new HDInsightVersion { Version = "1.2", VersionStatus = VersionStatus.Obsolete });
            versions.Add(new HDInsightVersion { Version = "1.5", VersionStatus = VersionStatus.Obsolete });
            versions.Add(new HDInsightVersion { Version = "1.6", VersionStatus = VersionStatus.Compatible });
            versions.Add(new HDInsightVersion { Version = "2.1", VersionStatus = VersionStatus.Compatible });
            foreach (HDInsightVersion version in versions)
            {
                Assert.IsTrue(
                    capabilities.Any(
                        capability =>
                        capability.Versions.Any(capVersion => string.Equals(version.Version, capVersion.Version, StringComparison.Ordinal))),
                    "unable to find version '{0}' in capabilities",
                    version);
            }

            var locations = new Collection<string> { "East US", "East US 2", "West US", "North Europe" };
            foreach (string location in locations)
            {
                Assert.IsTrue(
                    capabilities.Any(
                        capability => capability.Locations.Any(capLocation => string.Equals(location, capLocation, StringComparison.Ordinal))),
                    "unable to find location '{0}' in capabilities",
                    location);
            }
        }
    }
}
