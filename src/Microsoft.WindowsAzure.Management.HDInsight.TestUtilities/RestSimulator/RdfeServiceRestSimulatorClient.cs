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
namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight;

    internal class RdfeServiceRestSimulatorClient : IRdfeServiceRestClient
    {
        private IHDInsightCertificateCredential credentials;
        private readonly IntegrationTestManager testmanager = new IntegrationTestManager();
        private readonly IAbstractionContext context;

        public RdfeServiceRestSimulatorClient(IHDInsightCertificateCredential credentials, IAbstractionContext context)
        {
            var validCreds = IntegrationTestBase.GetValidCredentials() as IHDInsightCertificateCredential;
            if (validCreds == null || (credentials.Certificate.SubjectName != validCreds.Certificate.SubjectName && credentials.SubscriptionId != validCreds.SubscriptionId))
            {
                throw new HttpLayerException(HttpStatusCode.Unauthorized, "User " + validCreds.SubscriptionId + " is not authorized");
            }

            this.context = context;
            this.credentials = credentials;
        }

        public Task<IEnumerable<KeyValuePair<string, string>>> GetResourceProviderProperties()
        {
            var testCredential =
                this.testmanager.GetAllCredentials()
                           .FirstOrDefault(cred => cred.SubscriptionId == this.credentials.SubscriptionId);

            if (testCredential == null)
            {
                testCredential = this.testmanager.GetCredentials("default");
            }

            return
                Task.FromResult(
                    IntegrationTestBase.TestCredentials.ResourceProviderProperties.Select(
                        property => new KeyValuePair<string, string>(property.Key, property.Value)).AsEnumerable());
        }

        public IEnumerable<KeyValuePair<string, string>> ParseCapabilities(string payload)
        {
            // Open the XML.
            XDocument xdoc = XDocument.Parse(payload);
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            if (xdoc.Root == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            // Loops through the ResourceProviderProperty elements and extract the values for elements with "CAPABILITY_REGION" keys
            var capabilities = from element in xdoc.Root.Elements(ns + "ResourceProviderProperty")
                               let key = element.Element(ns + "Key")
                               let value = element.Element(ns + "Value")
                               where key != null && value != null
                               select new KeyValuePair<string, string>(key.Value, value.Value);

            return capabilities.ToList();
        }
    }
}
