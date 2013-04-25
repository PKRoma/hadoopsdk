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

using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Hadoop.WebClient.Common;

namespace Microsoft.Hadoop.WebClient.AmbariClient
{
    public class AmbariHttpClient
    {
        private HttpClient client;

        public AmbariHttpClient(Uri baseUri, string username, string password)
        {
            client = new HttpClient();
            Initialize(baseUri, username, password, username, null);
        }

        public AmbariHttpClient(Uri baseUri, string username, string password, string hadoopUserName)
        {
            client = new HttpClient();
            Initialize(baseUri, username, password, hadoopUserName, null);
        }

        public AmbariHttpClient(Uri baseUri, string username, string password, string hadoopUserName, HttpMessageHandler handler)
        {
            Initialize(baseUri, username, password, hadoopUserName, handler);
        }

        private void Initialize(Uri baseUri, string username, string password, string hadoopUserName, HttpMessageHandler handler)
        {
            // TODO - have version passed in

            client = handler == null ? new HttpClient() : new HttpClient(handler);
            client.BaseAddress = new Uri(baseUri, AmbariResources.RelativePath);
            if (username != null && password != null)
            {
                var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task<HttpResponseMessage> GetClusters()
        {
            return await client.SendAsyncRequestForNewJob(HttpMethod.Get, string.Empty, null);
        }

        public async Task<HttpResponseMessage> GetAsvMetrics(string storageAccount, DateTime start, DateTime end)
        {
            string metricUrl = AmbariUrlBuilder.GetAsvMetricsUrl(storageAccount, start, end);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Get, metricUrl, null);
        }

        public async Task<HttpResponseMessage> GetHostComponentMetric(string clusterName, string headnodeName)
        {
            string metricUrl = AmbariUrlBuilder.GetGetHostComponentMetricUrl(clusterName, headnodeName);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Get, metricUrl, null);
        }


    }
}
