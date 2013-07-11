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

namespace Microsoft.WindowsAzure.Management.Framework.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    internal class HttpClientAbstraction : DisposableObject, IHttpClientAbstraction
    {
        private HttpClient client;

        internal HttpClientAbstraction(HttpClient client)
        {
            this.client = client;
            this.Timeout = new TimeSpan(0, 5, 0);
            this.Method = HttpMethod.Get;
            this.RequestHeaders = new Dictionary<string, string>();
            this.ContentType = HttpHardcodes.ApplicationXml;
        }

        public TimeSpan Timeout { get; set; }

        public HttpMethod Method { get; set; }

        public Uri RequestUri { get; set; }

        public string Content { get; set; }

        public IDictionary<string, string> RequestHeaders { get; private set; }

        public string ContentType { get; set; }

        public async Task<IHttpResponseMessageAbstraction> SendAsync()
        {
            var requestMessage = new HttpRequestMessage();

            requestMessage.Method = this.Method;
            requestMessage.RequestUri = this.RequestUri;
            if (this.Method == HttpMethod.Post || this.Method == HttpMethod.Put)
            {
                requestMessage.Content = new StringContent(this.Content);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
            }
            requestMessage.Headers.Clear();
            foreach (KeyValuePair<string, string> header in this.RequestHeaders)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
            this.client.Timeout = this.Timeout;

            var result = await this.client.SendAsync(requestMessage);
            string content = null;
            if (result.Content.IsNotNull())
            {
                content = result.Content.ReadAsStringAsync().WaitForResult();
            }
            return new HttpResponseMessageAbstraction(result.StatusCode, new HttpResponseHeadersAbstraction(result.Headers), content);
        }

        public static IHttpClientAbstraction Create(X509Certificate2 cert)
        {
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(cert);
            return Help.SaveCreate(() => new HttpClientAbstraction(Help.SaveCreate(() => new HttpClient(handler))));
        }

        public static IHttpClientAbstraction Create()
        {
            return Help.SaveCreate(() => new HttpClientAbstraction(Help.SaveCreate<HttpClient>()));
        }
    }
}
