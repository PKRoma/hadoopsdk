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
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Hadoop.WebClient.Common;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace Microsoft.Hadoop.WebHCat.Protocol
{
    internal enum HadoopJobStatus
    {
        Running = 1,
        Succeded = 2,
        Failed = 3,
        Prep = 4
    }

    public class WebHCatHttpClient
    {
        private string hadoopUserName;

        private HttpClient client = new HttpClient();

        public TimeSpan Timeout
        {
            get { return this.client.Timeout; }
            set { this.client.Timeout = value; }
        }

        public WebHCatHttpClient(Uri baseUri, string username, string password)
        {
            Initialize(baseUri, username, password, username, null);
        }

        public WebHCatHttpClient(Uri baseUri, string username, string password, string hadoopUserName)
        {
            Initialize(baseUri, username, password, hadoopUserName, null);
        }

        public WebHCatHttpClient(Uri baseUri, string username, string password, string hadoopUserName, HttpMessageHandler handler)
        {
            Initialize(baseUri, username, password, hadoopUserName, handler);
        }

        private void Initialize(Uri baseUri, string username, string password, string hadoopUserName, HttpMessageHandler handler)
        {
            // TODO - have version passed in
            if (handler != null)
            {
                client = new HttpClient(handler);
            }
            this.hadoopUserName = hadoopUserName;
            client.BaseAddress = new Uri(baseUri, WebHCatResources.RelativeWebHCatPath);
            if (username != null && password != null)
            {
                var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public string HadoopUserName
        {
            get { return this.hadoopUserName; }
        }

        public async Task<HttpResponseMessage> CreateHiveJob(string execute, IEnumerable<string> file, IEnumerable<KeyValuePair<string, string>> defines, string statusDirectory, string callback)
        {
            var values = new List<KeyValuePair<string, string>>() { { new KeyValuePair<string, string>(WebHCatResources.Execute, execute) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.StatusDirectory, statusDirectory) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Callback, callback) }};

            values.AddRange(this.BuildList(WebHCatResources.File, file));
            values.AddRange(BuildNameValueList(WebHCatResources.Define, defines));
            return await SendAsyncRequest(HttpMethod.Post, WebHCatResources.Hive, values);
        }

        public async Task<HttpResponseMessage> CreatePigJob(string execute, IEnumerable<string> file, IEnumerable<string> args, IEnumerable<string> files, string statusDirectory, string callback)
        {
            var values = new List<KeyValuePair<string, string>>() { { new KeyValuePair<string, string>(WebHCatResources.Execute, execute) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.StatusDirectory, statusDirectory) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Callback, callback) }};
            if (files != null && files.Count() > 0)
            {
                values.Add(new KeyValuePair<string, string>(WebHCatResources.Files, BuildCommaSeparatedList(files)));
            }
            
            values.AddRange(this.BuildList(WebHCatResources.File, file));
            values.AddRange(this.BuildList(WebHCatResources.Arg, args));
            return await SendAsyncRequest(HttpMethod.Post, WebHCatResources.Pig, values);
        }

        public async Task<HttpResponseMessage> CreateMapReduceJarJob(string jar, 
                                                                     string className, 
                                                                     IEnumerable<string> libjars, 
                                                                     IEnumerable<string> files, 
                                                                     IEnumerable<string> args, 
                                                                     IEnumerable<KeyValuePair<string, string>> defines, 
                                                                     string statusDirectory, 
                                                                     string callback)
        {
            var values = new List<KeyValuePair<string, string>>() { { new KeyValuePair<string, string>(WebHCatResources.Jar, jar) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Class, className) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.StatusDirectory, statusDirectory) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Callback, callback)}};

            if (libjars != null && libjars.Count() > 0)
            {
                values.Add(new KeyValuePair<string, string>(WebHCatResources.Libjars, BuildCommaSeparatedList(libjars)));
            }

            if (files != null && files.Count() > 0)
            {
                values.Add(new KeyValuePair<string, string>(WebHCatResources.Files, BuildCommaSeparatedList(files)));
            }
            
            values.AddRange(this.BuildList(WebHCatResources.Arg, args));
            values.AddRange(BuildNameValueList(WebHCatResources.Define, defines));
            return await SendAsyncRequest(HttpMethod.Post, WebHCatResources.MapReduceJar, values);
        }

        public async Task<HttpResponseMessage> CreateMapReduceStreamingJob(string inputLocation, 
                                                                           string outputLocation, 
                                                                           string mapperLocation, 
                                                                           string reducerLocation, 
                                                                           IEnumerable<string> file,
                                                                           IEnumerable<KeyValuePair<string, string>> defines, 
                                                                           IEnumerable<string> files, 
                                                                           IEnumerable<KeyValuePair<string, string>> cmdenvs, 
                                                                           IEnumerable<string> args, 
                                                                           string statusDirectory, 
                                                                           string callback)
        {
            var values = new List<KeyValuePair<string, string>>() { { new KeyValuePair<string, string>(WebHCatResources.Input, inputLocation) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Output, outputLocation) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Mapper, mapperLocation) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Reducer, reducerLocation) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.StatusDirectory, statusDirectory) }, 
                                                                    { new KeyValuePair<string, string>(WebHCatResources.Callback, callback) }};

            //if (files != null && files.Count() > 0)
            //{
            //    values.Add(new KeyValuePair<string, string>(WebHCatResources.Files, BuildCommaSeparatedList(files)));
            //}

            if (files != null && files.Count() > 0)
            {
                values.Add(new KeyValuePair<string, string>(WebHCatResources.Files, BuildCommaSeparatedList(files)));
            }
            values.AddRange(this.BuildList(WebHCatResources.Arg, args));
            values.AddRange(BuildNameValueList(WebHCatResources.Cmdenv, cmdenvs));
            values.AddRange(BuildNameValueList(WebHCatResources.Define, defines));
            return await SendAsyncRequest(HttpMethod.Post, WebHCatResources.MapReduceStreaming, values);
        }

        public async Task<HttpResponseMessage> GetStatus()
        {
            return await SendAsyncRequest(HttpMethod.Get, WebHCatResources.Status, null);
        }

        public async Task<HttpResponseMessage> GetVersion()
        {
            return await SendAsyncRequest(HttpMethod.Get, WebHCatResources.Version, null);
        }

        public async Task<HttpResponseMessage> GetResponseTypes(string version)
        {
            return await SendAsyncRequest(HttpMethod.Get, "", null);
        }

        public async Task<HttpResponseMessage> GetQueue(string jobID)
        {
            return await SendAsyncRequest(HttpMethod.Get, WebHCatResources.Queue + "/" + jobID, null);
        }

        public async Task<HttpResponseMessage> GetQueue()
        {
            return await SendAsyncRequest(HttpMethod.Get, WebHCatResources.Queue, null);
        }

        public async Task<HttpResponseMessage> DeleteJob(string jobID)
        {
            return await SendAsyncRequest(HttpMethod.Delete, WebHCatResources.Queue + "/" + jobID, null);
        }

        public async Task<bool> HasJobCompleted(string jobID)
        {
            var response = await GetQueue(jobID);
            response.EnsureSuccessStatusCode();
            var job = await response.Content.ReadAsAsync<JObject>();
            var runState = job["status"]["runState"].Value<int>();
            var completed = job["completed"].ToString().Equals("done", StringComparison.OrdinalIgnoreCase);
            return (runState == (int)HadoopJobStatus.Failed || completed);
        }

        public async Task WaitForJobToCompleteAsync(string jobID)
        {
            while (true)
            {
                if (await HasJobCompleted(jobID))
                    break;
                System.Threading.Thread.Sleep(5000);
            }
            return;
        }

        private string UserNameParam()
        {
            return "?" + WebHCatResources.UserName + "=" + this.hadoopUserName;
        }

        private async Task<HttpResponseMessage> SendAsyncRequest(HttpMethod method, string requestUri, List<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null && (method == HttpMethod.Post || method == HttpMethod.Put))
            {
                throw new InvalidOperationException("Attempt to perform a post or put with no parameters.");
            }

            if (method == HttpMethod.Get || method == HttpMethod.Delete)
            {
                requestUri = requestUri + UserNameParam();
            }
            else
            {
                parameters.Add(new KeyValuePair<string, string>(WebHCatResources.UserName, this.hadoopUserName));
            }

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(WebHCatResources.ApplicationJson));
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                var content = new FormUrlEncodedContent(parameters.Where(kvp => kvp.Value != null));
                request.Content = content;
            }
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            
            response.EnsureSuccessStatusCode();
            return response;
        }

        private IEnumerable<KeyValuePair<string, string>> BuildNameValueList(string paramName, IEnumerable<KeyValuePair<string, string>> nameValuePairs)
        {
            if (nameValuePairs == null)
                yield break;

            foreach (var kvp in nameValuePairs)
            {
                yield return new KeyValuePair<string, string>(paramName, kvp.Key + "=" + kvp.Value);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> BuildList(string type, IEnumerable<string> args)
        {
            if (args == null)
                yield break;

            foreach (var arg in args )
            {
                yield return new KeyValuePair<string, string>(type, arg);
            }
        }

        private string BuildCommaSeparatedList(IEnumerable<string> input)
        {
            return string.Join(",", input.ToArray());
        }
    }
}
