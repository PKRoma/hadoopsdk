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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Hadoop.WebClient.Common;

namespace Microsoft.Hadoop.WebClient.OozieClient
{
    public class OozieHttpClient
    {
        private readonly HttpClient client;

        public OozieHttpClient(Uri baseUri, string username, string password)
        {
            client = new HttpClient();
            Initialize(baseUri, username, password);
        }

        public OozieHttpClient(Uri baseUri, string username, string password, HttpMessageHandler handler)
        {
            client = new HttpClient(handler);
            Initialize(baseUri, username, password);
        }

        private void Initialize(Uri baseUri, string username, string password)
        {
            client.BaseAddress = new Uri(baseUri, OozieResources.RelativePath);
            if (username != null && password != null)
            {
                var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task<HttpResponseMessage> GetStatus()
        {
            return await client.SendAsyncRequestForNewJob(HttpMethod.Get, OozieResources.Status, new Dictionary<string, string>() { });
        }

        public async Task<HttpResponseMessage> SubmitJob(IEnumerable<KeyValuePair<string, string>> properties)
        {
            string element = BuildXml(properties);
            return await client.SendXmlAsyncRequestForNewJob(HttpMethod.Post, OozieResources.Jobs, element);
        }

        private string BuildXml(IEnumerable<KeyValuePair<string, string>> properties)
        {
            var declaration = new XDeclaration("1.0", "utf-8", "yes");
            var element = new XElement("configuration");
            foreach (var pair in properties)
            {
                element.Add(new XElement("property", new XElement("name", pair.Key), new XElement("value", pair.Value)));
            }
            var builder = new StringBuilder();
            builder.AppendLine(declaration.ToString());
            builder.AppendLine(element.ToString());
            return builder.ToString();
        }

        public async Task<HttpResponseMessage> StartJob(string jobId)
        {
            var job = OozieUrlBuilder.GetJobActionUrl(jobId, OozieResources.ActionStart);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Put, job, new Dictionary<string, string>());
        }

        public async Task<HttpResponseMessage> KillJob(string jobId)
        {
            var job = OozieUrlBuilder.GetJobActionUrl(jobId, OozieResources.ActionKill);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Put, job, new Dictionary<string, string>());
        }

        public async Task<HttpResponseMessage> SuspendJob(string jobId)
        {
            var job = OozieUrlBuilder.GetJobActionUrl(jobId, OozieResources.ActionSuspend);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Put, job, new Dictionary<string, string>());
        }

        public async Task<HttpResponseMessage> ResumeJob(string jobId)
        {
            var job = OozieUrlBuilder.GetJobActionUrl(jobId, OozieResources.ActionResume);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Put, job, new Dictionary<string, string>());
        }

        public async Task<HttpResponseMessage> GetJobInfo(string jobId)
        {
            var job = OozieUrlBuilder.GetJobShowUrl(jobId, OozieResources.ShowInfo);
            return await client.SendAsyncRequestForNewJob(HttpMethod.Get, job, new Dictionary<string, string>());
        }

    }
}
