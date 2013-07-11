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

namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.RestClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    internal class HDInsightJobSubmissionRestClient : DisposableObject, IHDInsightJobSubmissionRestClient
    {
        private readonly IConnectionCredentials credentials;

        public HDInsightJobSubmissionRestClient(IConnectionCredentials credentials)
        {
            this.credentials = credentials;
        }

        // Method = "GET", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/hdinsight/~/containers/{containerName}/jobs"
        public async Task<string> ListJobs(string containerName, string location)
        {
            // Creates an HTTP client
            var resolver = ServiceLocator.Instance.Locate<ICloudServiceNameResolver>();
            using (
                IHttpClientAbstraction client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                string regionCloudServicename = resolver.GetCloudServiceName(
                    this.credentials.SubscriptionId, this.credentials.DeploymentNamespace, location);

                string relativeUri = string.Format(
                    "{0}/cloudservices/{1}/resources/{2}/~/containers/{3}/jobs",
                    this.credentials.SubscriptionId,
                    regionCloudServicename,
                    this.credentials.DeploymentNamespace,
                    containerName);

                client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));
                client.Method = HttpMethod.Get;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.SchemaVersion2);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);

                IHttpResponseMessageAbstraction httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HDInsightRestClientException(httpResponse.StatusCode, httpResponse.Content);
                }
                return httpResponse.Content;
            }
        }

        // Method = "GET", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/hdinsight/~/containers/{containerName}/jobs/{jobId}"
        public async Task<string> GetJobDetail(string containerName, string location, string jobId)
        {
            // Creates an HTTP client
            var resolver = ServiceLocator.Instance.Locate<ICloudServiceNameResolver>();
            using (
                IHttpClientAbstraction client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                string regionCloudServicename = resolver.GetCloudServiceName(
                    this.credentials.SubscriptionId, this.credentials.DeploymentNamespace, location);

                string relativeUri = string.Format(
                    "{0}/cloudservices/{1}/resources/{2}/~/containers/{3}/jobs/{4}",
                    this.credentials.SubscriptionId,
                    regionCloudServicename,
                    this.credentials.DeploymentNamespace,
                    containerName,
                    jobId);

                client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));
                client.Method = HttpMethod.Get;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.SchemaVersion2);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);

                IHttpResponseMessageAbstraction httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HDInsightRestClientException(httpResponse.StatusCode, httpResponse.Content);
                }
                return httpResponse.Content;
            }
        }

        // Method = "PUT", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/hdinsight/~/containers/{containerName}/jobs"
        public async Task<string> CreateJob(string containerName, string location, string payLoad)
        {
            // Creates an HTTP client
            var resolver = ServiceLocator.Instance.Locate<ICloudServiceNameResolver>();
            using (IHttpClientAbstraction client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(this.credentials.Certificate))
            {
                string regionCloudServicename = resolver.GetCloudServiceName(
                    this.credentials.SubscriptionId, this.credentials.DeploymentNamespace, location);

                string relativeUri = string.Format(
                    "{0}/cloudservices/{1}/resources/{2}/~/containers/{3}/jobs",
                    this.credentials.SubscriptionId,
                    regionCloudServicename,
                    this.credentials.DeploymentNamespace,
                    containerName);

                client.RequestUri = new Uri(this.credentials.Endpoint, new Uri(relativeUri, UriKind.Relative));
                client.Method = HttpMethod.Put;
                client.RequestHeaders.Add(HDInsightRestHardcodes.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestHardcodes.SchemaVersion2);
                client.RequestHeaders.Add(HDInsightRestHardcodes.Accept);
                client.Content = payLoad;

                IHttpResponseMessageAbstraction httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HDInsightRestClientException(httpResponse.StatusCode, httpResponse.Content);
                }
                return httpResponse.Content;
            }
        }
    }
}
