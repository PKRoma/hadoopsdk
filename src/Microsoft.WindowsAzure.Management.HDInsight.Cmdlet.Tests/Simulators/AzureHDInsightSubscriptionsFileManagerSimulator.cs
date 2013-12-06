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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    internal class AzureHDInsightSubscriptionsFileManagerSimulator : IAzureHDInsightSubscriptionsFileManager
    {
        private const string sampleSettingsFile =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<ProfileData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common\">" +
            "  <DefaultEnvironmentName>AzureCloud</DefaultEnvironmentName>" + "  <Environments /> " + "  <Subscriptions>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>true</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate>D569BF0D91A4010D20FB3BE980B389CB5740F4BE</ManagementCertificate> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>Hadoop</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>03453212-1457-4952-8b31-687db8e28af2</SubscriptionId> " + "    </AzureSubscriptionData>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate>8577FB6163323B956B83377821E4C755A9CD7675</ManagementCertificate> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " +
            "      <Name>HDI Experience RDFE - Windows Azure Internal Consumption</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>bbad5395-302e-4c60-a071-9921fd7cabb7</SubscriptionId> " + "    </AzureSubscriptionData>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate>55AE8C51E9113CBC13854833A02A48F3AB922D7B</ManagementCertificate> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " +
            "      <Name>HDI Experience SDK - Windows Azure Internal Consumption</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>1588b2ba-5646-4a39-b6b5-a753857fdbc3</SubscriptionId> " + "    </AzureSubscriptionData>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>HDInsightShared_07_547371</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>8f847984-9085-44ed-80bd-499c0f16a48c</SubscriptionId> " + "    </AzureSubscriptionData>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate>F3632ED3FAA74E2DEAD859787DDCE5FFFAC9E83B</ManagementCertificate> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>fake subscription</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>fake-subscription</SubscriptionId> " + "    </AzureSubscriptionData>" + "    <AzureSubscriptionData>" +
            "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementCertificate i:nil=\"true\" /> " +
            "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " + "      <Name>Null_Management_Certificate</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>96B8BEAF-6498-4F5E-853B-522474A64BF8</SubscriptionId> " + "    </AzureSubscriptionData>" +
            "    <AzureSubscriptionData>" + "      <ActiveDirectoryEndpoint i:nil=\"true\" /> " + "      <ActiveDirectoryTenantId i:nil=\"true\" /> " +
            "      <ActiveDirectoryUserId i:nil=\"true\" /> " + "      <CloudStorageAccount i:nil=\"true\" /> " + "      <IsDefault>false</IsDefault>" +
            "      <LoginType i:nil=\"true\" /> " + "      <ManagementEndpoint>https://management.core.windows.net/</ManagementEndpoint> " +
            "      <Name>No_Management_Certificate</Name> " +
            "      <RegisteredResourceProviders xmlns:d4p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" /> " +
            "      <SubscriptionId>96B8BEAF-6498-4F5E-853B-522474A64BF8</SubscriptionId> " + "    </AzureSubscriptionData>" + "  </Subscriptions>" +
            "</ProfileData>";

        public void Dispose()
        {
        }

        public IEnumerable<SubscriptionData> GetSubscriptions()
        {
            return this.LoadSubscriptionData();
        }

        internal IEnumerable<SubscriptionData> LoadSubscriptionData()
        {
            var subscriptions = new List<SubscriptionData>();
            byte[] settingsFileBytes = Encoding.UTF8.GetBytes(sampleSettingsFile);
            using (var memoryStream = new MemoryStream(settingsFileBytes))
            {
                using (var azureSubscriptionFileManager = new AzureHDInsightSubscriptionsFileManager())
                {
                    subscriptions.AddRange(azureSubscriptionFileManager.LoadSubscriptionData(memoryStream));
                }
            }

            subscriptions.Add(
                new SubscriptionData
                {
                    Certificate = new X509Certificate2(IntegrationTestBase.TestCredentials.Certificate),
                    SubscriptionId = IntegrationTestBase.TestCredentials.SubscriptionId,
                    SubscriptionName = "HDInsightShared_02_547371"
                });

            return subscriptions;
        }
    }
}
