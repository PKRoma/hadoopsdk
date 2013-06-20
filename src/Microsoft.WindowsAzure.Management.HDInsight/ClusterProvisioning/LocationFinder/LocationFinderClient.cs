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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class LocationFinderClient : ILocationFinderClient
    {
        private readonly IConnectionCredentials credentials;

        internal LocationFinderClient(IConnectionCredentials credentials)
        {
            this.credentials = credentials;
        }
        
        // Method = "GET", UriTemplate = "UriTemplate = "{subscriptionId}/resourceproviders/{resourceProviderNamespace}/Properties?resourceType={resourceType}"
        public async Task<Collection<string>> ListAvailableLocations()
        {
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                Guid subscriptionId = this.credentials.SubscriptionId;
                string cloudServiceName = this.credentials.DeploymentNamespace;
                string relativeUri = string.Format(CultureInfo.InvariantCulture,
                                                    "{0}/resourceproviders/{1}/Properties?resourceType={2}",
                                                    subscriptionId,
                                                    this.credentials.DeploymentNamespace,
                                                    "containers");
                client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));

                client.Method = HttpMethod.Get;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);

                using (var httpResponse = await client.SendAsync())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HDInsightRestClientException(httpResponse.StatusCode, httpResponse.Content);
                    }

                    return ParseLocations(httpResponse.Content);
                }
            }
        }

        internal static Collection<string> ParseLocations(string payload)
        {
            // Open the XML.
            XDocument xdoc = XDocument.Parse(payload);
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            if (xdoc.Root == null)
            {
                return new Collection<string>();
            }

            // Loops through the ResourceProviderProperty elements and extract the values for elements with "CAPABILITY_REGION" keys
            var result = new Collection<string>();
            foreach (var element in xdoc.Root.Elements(ns + "ResourceProviderProperty"))
            {
                var key = element.Elements(ns + "Key").FirstOrDefault();
                var value = element.Elements(ns + "Value").FirstOrDefault();
                if (key == null || key.Value == null || value == null || value.Value == null)
                {
                    continue;
                }

                if (key.Value.StartsWith("CAPABILITY_REGION", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(value.Value);
                }
            }

            return result;
        }
    }
}