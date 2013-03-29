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

namespace Microsoft.Hadoop.WebClient.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.WebHDFS;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Class that can be used to acess Hdfs using the WebHdfs REST service
    /// </summary>
    public class WebHdfsHttpClient : IWebHdfsHttpClient
    {
        /// <summary>
        /// Gets or sets the handler to use when creating an HttpClient.
        /// </summary>
        internal WebRequestHandler RequestHandler { get; set; }

        /// <summary>
        /// Gets or Sets Uri for the WebHdfs endpoint.
        /// </summary>
        internal Uri WebHdfsUri { get; set; }

        /// <summary>
        /// Intializes a new instance of the WebHdfsHttpClient class.
        /// </summary>
        /// <param name="webHdfsUri">The Uri for the WebHdfs endpoint.</param>
        public WebHdfsHttpClient(Uri webHdfsUri)
        {
            this.WebHdfsUri = webHdfsUri;
        }

        /// <summary>
        /// Method that creates an HttpClient to use when communicating with WebHdfs.
        /// </summary>
        /// <param name="allowsAutoRedirect">Allows the client to automatically redirect the user.</param>
        /// <returns>An Http Client for making requests.</returns>
        private HttpClient CreateHttpClient(bool allowsAutoRedirect = true)
        {
            if (RequestHandler != null)
            {
                return new HttpClient(RequestHandler);
            }
            return new HttpClient(new WebRequestHandler() { AllowAutoRedirect = allowsAutoRedirect });
        }

        internal Uri CreateRequestUri(WebHdfsOperation operation, string path, List<KeyValuePair<string, string>> parameters)
        {
            var paramString = string.Empty;
            if (parameters != null)
            {
                paramString = parameters.Aggregate("", (current, param) => current + string.Format("&{0}={1}", param.Key, param.Value));
            }
            var queryString = string.Format("{0}?op={1}{2}", path, operation, paramString);
            return new Uri(WebHdfsUri + queryString);
        }

        /// <inheritdocs/>
        public async Task<HttpContent> OpenFile(string path)
        {
            var client = this.CreateHttpClient();
            var resp = await client.GetAsync(this.CreateRequestUri(WebHdfsOperation.OPEN, path, null));
            resp.EnsureSuccessStatusCode();
            return resp.Content;
        }

        /// <inheritdocs/>
        public async Task<string> CreateFile(string path, Stream content, bool overwrite)
        {
            var client = this.CreateHttpClient(false);

            var parameters = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("overwrite", overwrite.ToString()) };
            var redir = await client.PutAsync(this.CreateRequestUri(WebHdfsOperation.CREATE, path, parameters), null);

            content.Position = 0;
            var fileContent = new StreamContent(content);
            var create = await client.PutAsync(redir.Headers.Location, fileContent);
            create.EnsureSuccessStatusCode();
            return create.Headers.Location.ToString();
        }

        /// <inheritdocs/>
        public async Task<bool> Delete(string path, bool recursive)
        {
            var client = this.CreateHttpClient();

            var parameters = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("recursive", recursive.ToString()) };
            var drop = await client.DeleteAsync(this.CreateRequestUri(WebHdfsOperation.DELETE, path, parameters));
            drop.EnsureSuccessStatusCode();

            var content = await drop.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");
        }

        /// <inheritdocs/>
        public async Task<DirectoryEntry> GetFileStatus(string path)
        {
            var client = this.CreateHttpClient();

            var status = await client.GetAsync(this.CreateRequestUri(WebHdfsOperation.GETFILESTATUS, path, null));
            status.EnsureSuccessStatusCode();

            var filesStatusTask = await status.Content.ReadAsAsync<JObject>();

            return new DirectoryEntry(filesStatusTask.Value<JObject>("FileStatus"));
        }
    }
}
