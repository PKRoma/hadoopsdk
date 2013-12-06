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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml.Linq;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;

    internal class AzureHDInsightSubscriptionsFileManager : IAzureHDInsightSubscriptionsFileManager
    {
        private const string AzureDirectoryName = "Windows Azure Powershell";
        private const string AzureSettingsFileName = "WindowsAzureProfile.xml";
        private const string AzureSettingsFileXmlNs = "http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common";

        private const string AzureToolsInstallAndConfigureLocation = "http://go.microsoft.com/fwlink/?linkid=325564";

        private const string AzureToolsNotInstalled =
            "Please install Windows Azure Powershell Tools v 0.7.0 or higher and re-import Azure publish settings file according to instructions here {0}.\r\nNewer version of Azure PowerShell is required to enable auto detection of Certificates for Azure Subscriptions.";

        private readonly string azureSettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AzureDirectoryName, AzureSettingsFileName);

        public void Dispose()
        {
        }

        public IEnumerable<SubscriptionData> GetSubscriptions()
        {
            return this.LoadSubscriptionData();
        }

        internal static X509Certificate2 GetCertificateFromThumbprint(string thumbprint)
        {
            if (thumbprint.IsNullOrEmpty())
            {
                return null;
            }

            X509Certificate2 certificate;
            if (!TryFindCertificateInStore(StoreLocation.CurrentUser, thumbprint, out certificate))
            {
                TryFindCertificateInStore(StoreLocation.LocalMachine, thumbprint, out certificate);
            }

            return certificate;
        }

        internal static Guid GetSubscriptionId(XElement subscriptionId)
        {
            Guid resultGuid;
            if (subscriptionId.IsNull())
            {
                resultGuid = Guid.Empty;
            }
            else if (!Guid.TryParse(subscriptionId.Value, out resultGuid))
            {
                resultGuid = Guid.Empty;
            }
            return resultGuid;
        }

        internal IEnumerable<SubscriptionData> LoadSubscriptionData()
        {
            return this.LoadSubscriptionData(this.azureSettingsFilePath);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "linkid",
            Justification = "Needed to point to Azure tools download location.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "fwlink",
            Justification = "Needed to point to Azure tools download location.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Powershell",
            Justification = "Needed to point to Azure tools download location.")]
        internal IEnumerable<SubscriptionData> LoadSubscriptionData(string settingsFilePath)
        {
            settingsFilePath.ArgumentNotNullOrEmpty("settingsFilePath");

            if (File.Exists(settingsFilePath))
            {
                using (FileStream fileStream = File.OpenRead(settingsFilePath))
                {
                    return this.LoadSubscriptionData(fileStream);
                }
            }

            throw new NotSupportedException(
                string.Format(CultureInfo.InvariantCulture, AzureToolsNotInstalled, AzureToolsInstallAndConfigureLocation));
        }

        internal IEnumerable<SubscriptionData> LoadSubscriptionData(Stream settingsFileStream)
        {
            settingsFileStream.ArgumentNotNull("settingsFileStream");

            XDocument settingsDocument = XDocument.Load(settingsFileStream);
            IEnumerable<SubscriptionData> subscriptionData =
                from subscription in settingsDocument.Descendants(XName.Get("AzureSubscriptionData", AzureSettingsFileXmlNs))
                let endpoint = subscription.Element(XName.Get("ManagementEndpoint", AzureSettingsFileXmlNs))
                let subscriptionId = subscription.Element(XName.Get("SubscriptionId", AzureSettingsFileXmlNs))
                let subscriptionName = subscription.Element(XName.Get("Name", AzureSettingsFileXmlNs))
                let certificate = subscription.Element(XName.Get("ManagementCertificate", AzureSettingsFileXmlNs))
                select
                    new SubscriptionData
                    {
                        Endpoint = endpoint == null ? string.Empty : endpoint.Value,
                        SubscriptionId = GetSubscriptionId(subscriptionId),
                        SubscriptionName = subscriptionName == null ? string.Empty : subscriptionName.Value,
                        Certificate = certificate == null ? null : GetCertificateFromThumbprint(certificate.Value)
                    };

            return subscriptionData.ToList();
        }

        private static bool TryFindCertificateInStore(StoreLocation location, string thumbprint, out X509Certificate2 certificate)
        {
            certificate = null;
            var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            store.Close();
            if (certificates.Count > 0)
            {
                certificate = certificates[0];
            }

            return certificate != null;
        }
    }
}
