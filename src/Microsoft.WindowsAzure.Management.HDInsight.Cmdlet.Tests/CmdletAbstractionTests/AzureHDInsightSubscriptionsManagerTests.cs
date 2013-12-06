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
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class AzureHDInsightSubscriptionsManagerTests : IntegrationTestBase
    {
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
        public void CanLoadSubscriptionFromFileWithEmptySubscriptionId()
        {
            const string sampleSettingsFile =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
                "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
                "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" + "      <LoginType i:nil=\"true\" /> " +
                "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
                "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>fake subscription</Name> " +
                "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                "      <SubscriptionId></SubscriptionId> " + "    </AzureSubscriptionData>" + "  </Subscriptions>" + "</ProfileData>";
            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }
            Assert.AreEqual(subscriptions[0].SubscriptionId, Guid.Empty);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanLoadSubscriptionFromFileWithInvalidSubscriptionId()
        {
            const string sampleSettingsFile =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
                "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
                "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" + "      <LoginType i:nil=\"true\" /> " +
                "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
                "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>fake subscription</Name> " +
                "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                "      <SubscriptionId>shvohra-fakesubscription</SubscriptionId> " + "    </AzureSubscriptionData>" + "  </Subscriptions>" +
                "</ProfileData>";
            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }
            Assert.AreEqual(subscriptions[0].SubscriptionId, Guid.Empty);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanLoadSubscriptionFromFileWithNullSubscriptionId()
        {
            const string sampleSettingsFile =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
                "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
                "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" + "      <LoginType i:nil=\"true\" /> " +
                "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
                "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>fake subscription</Name> " +
                "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                "    </AzureSubscriptionData>" + "  </Subscriptions>" + "</ProfileData>";
            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }
            Assert.AreEqual(subscriptions[0].SubscriptionId, Guid.Empty);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanLoadSubscriptionFromFileWithValidAndInvalidSubscriptionId()
        {
            const string sampleSettingsFile =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
                "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
                "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>true</IsDefault>" + "      <LoginType i:nil=\"true\" /> " +
                "      <ManagementCertificate>D569BF0D91A4010D20FB3BE980B389CB5740F4BE</ManagementCertificate> " +
                "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>Hadoop</Name> " +
                "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                "      <SubscriptionId>03453212-1457-4952-8b31-687db8e28af2</SubscriptionId> " + "    </AzureSubscriptionData>" +
                "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" + "      <LoginType i:nil=\"true\" /> " +
                "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
                "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>fake subscription</Name> " +
                "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                "      <SubscriptionId>shvohra-fakesubscription</SubscriptionId> " + "    </AzureSubscriptionData>" + "  </Subscriptions>" +
                "</ProfileData>";
            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }
            Assert.AreEqual(subscriptions[0].SubscriptionId, new Guid("03453212-1457-4952-8b31-687db8e28af2"));
            Assert.AreEqual(subscriptions[1].SubscriptionId, Guid.Empty);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        public void CanParseNullMangementCertificate()
        {
            string sampleSettingsFile = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                        "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
                                        "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
                                        "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " +
                                        "      <ActiveDirectoryTenantId i:nil=\"true\" /> " + "      <ActiveDirectoryUserId i:nil=\"true\" /> " +
                                        "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
                                        "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate i:nil=\"true\" />" +
                                        "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " +
                                        "      <Name>HDInsightShared_07_547371</Name> " +
                                        "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
                                        "      <SubscriptionId>8f847984-9085-44ed-80bd-499c0f16a48c</SubscriptionId> " +
                                        "    </AzureSubscriptionData>" + "  </Subscriptions>" + "</ProfileData>";

            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }

            Assert.IsNull(subscriptions[0].Certificate);
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
                        "Please install Windows Azure Powershell Tools v 0.7.0 or higher and re-import Azure publish settings file according to instructions here http://go.microsoft.com/fwlink/?linkid=325564.\r\n" +
                        "Newer version of Azure PowerShell is required to enable auto detection of Certificates for Azure Subscriptions.",
                        nsException.Message);
                }
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
