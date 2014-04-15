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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.HadoopJobSubmissionRestCleint;
    using Microsoft.Hadoop.Client.WebHCatRest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight;

    internal class RdfeServiceRestClient : IRdfeServiceRestClient
    {
        private readonly IHDInsightSubscriptionCredentials credentials;
        private readonly HDInsight.IAbstractionContext context;
        private readonly bool ignoreSslErrors;

        internal RdfeServiceRestClient(IHDInsightSubscriptionCredentials credentials, HDInsight.IAbstractionContext context, bool ignoreSslErrors)
        {
            this.context = context;
            this.ignoreSslErrors = ignoreSslErrors;
            this.credentials = credentials;
        }

        internal async Task<IHttpResponseMessageAbstraction> ProcessGetResourceProviderPropertiesRequest(IHttpClientAbstraction client)
        {
            var httpLogic = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            client.Timeout = httpLogic.HttpOperationTimeout;
            Guid subscriptionId = this.credentials.SubscriptionId;
            string relativeUri = string.Format(CultureInfo.InvariantCulture,
                                                "{0}/resourceproviders/{1}/Properties?resourceType={2}",
                                                subscriptionId,
                                                this.credentials.DeploymentNamespace,
                                                "containers");
            client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));

            client.Method = HttpMethod.Get;
            client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
            client.RequestHeaders.Add(HDInsightRestConstants.Accept);

            var httpResponse = await client.SendAsync();
            return httpResponse;
        }

        // Method = "GET", UriTemplate = "UriTemplate = "{subscriptionId}/resourceproviders/{resourceProviderNamespace}/Properties?resourceType={resourceType}"
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetResourceProviderProperties()
        {
            int i = 0;
            var start = DateTime.UtcNow;
            var timingManager = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            var factory = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>();
            var result = await factory.Retry(this.credentials,
                                             this.context,
                                             this.ProcessGetResourceProviderPropertiesRequest,
                                             r => 
                                             { 
                                                 i++;
                                                 return r.StatusCode != HttpStatusCode.Accepted && r.StatusCode != HttpStatusCode.OK;
                                             },
                                             timingManager.RetryCount,
                                             timingManager.RetryInterval,
                                             this.ignoreSslErrors);

            if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpLayerException(result.StatusCode, result.Content, i, DateTime.UtcNow - start);
            }

            return this.ParseCapabilities(result.Content);
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
