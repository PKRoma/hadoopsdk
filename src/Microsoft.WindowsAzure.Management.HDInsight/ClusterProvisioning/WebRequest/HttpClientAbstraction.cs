namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;

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
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Method = this.Method;
            requestMessage.RequestUri = this.RequestUri;
            if (this.Method == HttpMethod.Post || this.Method == HttpMethod.Put)
            {
                requestMessage.Content = new StringContent(this.Content);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
            }
            requestMessage.Headers.Clear();
            foreach (var header in this.RequestHeaders)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
            this.client.Timeout = this.Timeout;

            var result = await this.client.SendAsync(requestMessage);
            return new HttpResponseMessageAbstraction(result);
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
