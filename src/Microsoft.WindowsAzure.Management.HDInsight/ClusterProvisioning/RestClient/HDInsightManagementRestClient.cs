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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class HDInsightManagementRestClient : DisposableObject, IHDInsightManagementRestClient
    {
        private readonly IConnectionCredentials credentials;

        internal HDInsightManagementRestClient(IConnectionCredentials credentials)
        {
            this.credentials = credentials;
        }
        
        // Method = "GET", UriTemplate = "{subscriptionId}/cloudservices"
        public async Task<string> ListCloudServices()
        {
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                // Creates the request
                string relativeUri = string.Format("{0}/cloudservices",
                                                    this.credentials.SubscriptionId);
                client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);
                client.Method = HttpMethod.Get;
                    
                // Sends, validates and parses the response
                using (var httpResponse = await client.SendAsync())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HDInsightRestClientException(httpResponse.StatusCode,
                                                                httpResponse.Content);
                    }
                    return httpResponse.Content;
                }
            }
        }

        // Method = "PUT", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task CreateResource(string resourceId, string resourceType, string location, string clusterPayload)
        {
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                string regionCloudServicename = GetCloudServiceName(this.credentials.SubscriptionId,
                                                                    this.credentials.DeploymentNamespace,
                                                                    location);

                string relativeUri = string.Format(CultureInfo.InvariantCulture,
                                                    "{0}/cloudservices/{1}/resources/{2}/{3}/{4}",
                                                    this.credentials.SubscriptionId,
                                                    regionCloudServicename,
                                                    this.credentials.DeploymentNamespace,
                                                    resourceType,
                                                    resourceId);

                client.RequestUri = new Uri(this.credentials.Endpoint,
                                            new Uri(relativeUri,
                                                    UriKind.Relative));
                client.Method = HttpMethod.Put;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);
                client.Content = clusterPayload;

                using (var httpResponse = await client.SendAsync())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        throw new HDInsightRestClientException(httpResponse.StatusCode,
                                                                httpResponse.Content);
                    }
                }
            }
        }

        // Method = "PUT", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task CreateContainer(string dnsName, string location, string clusterPayload)
        {
            await this.CreateResource(dnsName,
                                      "containers",
                                      location,
                                      clusterPayload);
        }

        // Method = "DELETE", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task DeleteContainer(string dnsName, string location)
        {
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                Guid subscriptionId = this.credentials.SubscriptionId;
                string cloudServiceName = this.credentials.DeploymentNamespace;
                string regionCloudServicename = GetCloudServiceName(subscriptionId,
                                                                    cloudServiceName,
                                                                    location);
                string relativeUri = string.Format(CultureInfo.InvariantCulture,
                                                    "{0}/cloudservices/{1}/resources/{2}/{3}/{4}",
                                                    subscriptionId,
                                                    regionCloudServicename,
                                                    this.credentials.DeploymentNamespace,
                                                    "containers",
                                                    dnsName);
                client.RequestUri = new Uri(this.credentials.Endpoint,
                                            new Uri(relativeUri, UriKind.Relative));

                client.Method = HttpMethod.Delete;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);

                using (var httpResponse = await client.SendAsync())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        throw new HDInsightRestClientException(httpResponse.StatusCode,
                                                                httpResponse.Content);
                    }
                }
            }
        }

        // TODO: REMOVE THIS AND HAVE A LOOKUP ON AZURE LOGIC
        internal static string GetCloudServiceName(Guid subscriptionId, string extensionPrefix, string region)
        {
            string hashedSubId = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                hashedSubId = Base32NoPaddingEncode(sha256.ComputeHash(Encoding.UTF8.GetBytes(subscriptionId.ToString())));
            }

            return string.Format(CultureInfo.InvariantCulture,
                                 "{0}{1}-{2}",
                                 extensionPrefix,
                                 hashedSubId,
                                 region.Replace(' ', '-'));
        }

        // TODO: REMOVE THIS AND HAVE A LOOKUP ON AZURE LOGIC
        private static string Base32NoPaddingEncode(byte[] data)
        {
            const string Base32StandardAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            var result = new StringBuilder(Math.Max((int)Math.Ceiling(data.Length * 8 / 5.0), 1));

            var emptyBuffer = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var workingBuffer = new byte[8];

            // Process input 5 bytes at a time
            for (int i = 0; i < data.Length; i += 5)
            {
                int bytes = Math.Min(data.Length - i, 5);
                Array.Copy(emptyBuffer, workingBuffer, emptyBuffer.Length);
                Array.Copy(data, i, workingBuffer, workingBuffer.Length - (bytes + 1), bytes);
                Array.Reverse(workingBuffer);
                ulong val = BitConverter.ToUInt64(workingBuffer, 0);

                for (int bitOffset = ((bytes + 1) * 8) - 5; bitOffset > 3; bitOffset -= 5)
                {
                    result.Append(Base32StandardAlphabet[(int)((val >> bitOffset) & 0x1f)]);
                }
            }

            return result.ToString();
        }
    }
}