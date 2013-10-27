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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml.Linq;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class AzureHDInsightSubscriptionsFileManager : IAzureHDInsightSubscriptionsFileManager
    {
        private const string AzureDirectoryName = "Windows Azure Powershell";
        private const string AzureSettingsFileXmlNs = "http://schemas.datacontract.org/2004/07/Microsoft.WindowsAzure.Commands.Utilities.Common";
        private const string AzureSettingsFileName = "WindowsAzureProfile.xml";
        private const string AzureToolsNotInstalled = "Please install Windows Azure Powershell Tools v 0.7.0 or higher from {0}.\r\nThis version is required to enable auto detection of Certificates for Azure Subscriptions.";
        private const string AzureToolsInstallLocation = "http://go.microsoft.com/?linkid=9811175&clcid=0x409";
        private readonly string azureSettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AzureDirectoryName, AzureSettingsFileName);

        public void Dispose()
        {
        }

        public IEnumerable<SubscriptionData> GetSubscriptions()
        {
            return this.LoadSubscriptionData();
        }

        internal IEnumerable<SubscriptionData> LoadSubscriptionData()
        {
            return this.LoadSubscriptionData(this.azureSettingsFilePath);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "linkid", Justification = "Needed to point to Azure tools download location.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Powershell", Justification = "Needed to point to Azure tools download location.")]
        internal IEnumerable<SubscriptionData> LoadSubscriptionData(string settingsFilePath)
        {
            settingsFilePath.ArgumentNotNullOrEmpty("settingsFilePath");

            if (File.Exists(settingsFilePath))
            {
                using (var fileStream = File.OpenRead(settingsFilePath))
                {
                    return this.LoadSubscriptionData(fileStream);
                }
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, AzureToolsNotInstalled, AzureToolsInstallLocation));
        }

        internal IEnumerable<SubscriptionData> LoadSubscriptionData(Stream settingsFileStream)
        {
            settingsFileStream.ArgumentNotNull("settingsFileStream");

            var settingsDocument = XDocument.Load(settingsFileStream);
            var subscriptionData = from subscription in settingsDocument.Descendants(XName.Get("AzureSubscriptionData", AzureSettingsFileXmlNs))
                                   let endpoint = subscription.Element(XName.Get("ManagementEndpoint", AzureSettingsFileXmlNs))
                                   let subscriptionId = subscription.Element(XName.Get("SubscriptionId", AzureSettingsFileXmlNs))
                                   let subscriptionName = subscription.Element(XName.Get("Name", AzureSettingsFileXmlNs))
                                   let certificate = subscription.Element(XName.Get("ManagementCertificate", AzureSettingsFileXmlNs))
                                   select new SubscriptionData()
                                   {
                                       Endpoint = endpoint == null ? string.Empty : endpoint.Value,
                                       SubscriptionId = subscriptionId == null ? Guid.Empty : GetGuidFromString(subscriptionId.Value),
                                       SubscriptionName = subscriptionName == null ? string.Empty : subscriptionName.Value,
                                       Certificate = certificate == null ? null : GetCertificateFromThumbprint(certificate.Value)
                                   };

            return subscriptionData.ToList();
        }

        internal static Guid GetGuidFromString(string guid)
        {
            return new Guid(guid);
        }

        internal static X509Certificate2 GetCertificateFromThumbprint(string thumbprint)
        {
            X509Certificate2 certificate;
            if (!TryFindCertificateInStore(StoreLocation.CurrentUser, thumbprint, out certificate))
            {
                TryFindCertificateInStore(StoreLocation.LocalMachine, thumbprint, out certificate);
            }

            return certificate;
        }

        private static bool TryFindCertificateInStore(StoreLocation location, string thumbprint, out X509Certificate2 certificate)
        {
            certificate = null;
            var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            store.Close();
            if (certificates.Count > 0)
            {
                certificate = certificates[0];
            }

            return certificate != null;
        }
    }
}
