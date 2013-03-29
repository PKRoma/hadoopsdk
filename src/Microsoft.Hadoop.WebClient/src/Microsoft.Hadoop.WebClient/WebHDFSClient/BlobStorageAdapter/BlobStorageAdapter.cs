using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS.Adapters
{
    using System.Globalization;
    using Microsoft.Hadoop.WebClient.WebHDFSClient.BlobStorageAdapter;

    public abstract class WebHDFSStorageAdapter : HttpMessageHandler
    {
        public abstract Uri BaseUri { get; }
    }

    public class BlobStorageAdapter : WebHDFSStorageAdapter
    {
        private IStorageAccountNameResolver accountNameResolver;

        public string Account
        {
            get { return this.accountNameResolver.AccountName; }
        }

        public string FullAccountName
        {
            get { return this.accountNameResolver.FullAccount; }
        }

        private string key;

        public string ContainerName { get; private set; }

        private BlobStorageClient blobStorageClient;


        internal BlobStorageAdapter(string account, string key)
        {
            this.accountNameResolver = new StorageAccountNameResolver(account);
            this.key = key;
        }

        public BlobStorageAdapter(string account, string key, string containerName, bool createContainerIfDoesNotExist)
            : this(account, key)
        {
            this.Connect(containerName, createContainerIfDoesNotExist);
        }

        public BlobStorageAdapter(string account, string key, string blobStorageAddress, string containerName, bool createContainerIfDoesNotExist)
            : this(account, key)
        {
            this.accountNameResolver = new StorageAccountNameResolver(account + "." + blobStorageAddress);
            this.Connect(containerName, createContainerIfDoesNotExist);
        }

        internal void Connect(string containerName, bool createContainerIfDoesNotExist)
        {
            this.ContainerName = containerName;
            this.blobStorageClient = new BlobStorageClient(this.accountNameResolver.FullAccountUrl.ToString(), this.Account, this.key);
            this.blobStorageClient.SetContainer(containerName, createContainerIfDoesNotExist);
        }

        public override Uri BaseUri
        {
            get { return this.accountNameResolver.FullAccountUrl; }
        }

        public void Disconnect()
        {
            this.blobStorageClient = null;
        }

        public void DeleteContainer()
        {
            this.blobStorageClient.DeleteContainer();
            this.ContainerName = null;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri uri = request.RequestUri;
            string op = uri.ParseQueryString()["op"];

            switch (op)
            {
                case "GETHOMEDIRECTORY":
                    return this.GetHomeDirectory(request, cancellationToken);
                case "CREATE":
                    return this.CreateFile(request, cancellationToken);
                case "MKDIRS":
                    return this.MakeDirs(request, cancellationToken);
                case "DELETE":
                    return this.Delete(request, cancellationToken);
                case "LISTSTATUS":
                    return this.ListStatus(request, cancellationToken);
                case "GETFILESTATUS":
                    return this.ListFileStatus(request, cancellationToken);
                case "OPEN":
                    return this.Open(request, cancellationToken);
                case "RENAME":
                    return this.Rename(request, cancellationToken);
                case "GETCONTENTSUMMARY":
                    return this.GetContentSummary(request, cancellationToken);
                case "GETFILECHECKSUM":
                    return this.GetFileChecksum(request, cancellationToken);
                default:
                    return this.BadRequest(request, cancellationToken);
            }
        }

        private Task<HttpResponseMessage> Open(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            // TODO (BUG) - fix to follow spec, should redirect
            string path = GetAzurePath(request.RequestUri.LocalPath);
            resp.StatusCode = HttpStatusCode.OK;
            string contentType;
            long length;

            var content = new MemoryStream();
            blobStorageClient.OpenFile(path, content, out contentType, out length);
            content.Position = 0;
            resp.Content = new StreamContent(content);
            resp.Content.Headers.ContentLength = length;
            MediaTypeHeaderValue v;
            MediaTypeHeaderValue.TryParse(contentType, out v);
            resp.Content.Headers.ContentType = v;
            return TaskEx.Run(() => resp);
        }

        private Task<HttpResponseMessage> BadRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return TaskEx.Run(() => new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        private Task<HttpResponseMessage> ListFileStatus(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            resp.StatusCode = HttpStatusCode.OK;
            string path = GetAzurePath(request.RequestUri.LocalPath);
            var desc = blobStorageClient.GetBlobDescriptor(path);
            if (desc == null)
            {
                throw new HttpRequestException("404 (Not Found)");
            }
            // TODO - need to add support for group, modificationtime(?), owner, premission 
            var content = new JObject(new JObject(new JProperty("FileStatus",
                        new JObject(
                            new JProperty("length", desc.Length), 
                            new JProperty("pathSuffix", desc.Name),
                            new JProperty("type", desc.Type == BlobType.Directory ? "DIRECTORY" : "FILE")))));

            resp.Content = new StringContent(content.ToString(), Encoding.Default, "application/json");

            return TaskEx.Run(() => resp);
        }

        private Task<HttpResponseMessage> ListStatus(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            resp.StatusCode = HttpStatusCode.OK;
            string path = GetAzurePath(request.RequestUri.LocalPath);

            var descriptors = blobStorageClient.ListDirectory(path);

            // TODO - need to add support for group, modificationtime(?), owner, premission 

            var content = new JObject(new JProperty("FileStatuses", 
                new JObject(new JProperty("FileStatus", 
                    new JArray(
                        from desc in descriptors
                        select new JObject(
                            new JProperty("length", desc.Length),
                            new JProperty("pathSuffix", desc.Name),
                            new JProperty("type", desc.Type == BlobType.Directory ? "DIRECTORY" : "FILE")))))));

            resp.Content = new StringContent(content.ToString(), Encoding.Default, "application/json");

            return TaskEx.Run(() => resp);
        }

        private async Task<HttpResponseMessage> CreateFile(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            // TODO - should figure out different way to determine if redirect happened.
            if (request.Content == null)
            {
                // redirect
                resp.StatusCode = HttpStatusCode.TemporaryRedirect;
                resp.Headers.Add("Location", request.RequestUri.ToString());
            }
            else
            {
                string path = GetAzurePath(request.RequestUri.LocalPath);
                Stream contents = await request.Content.ReadAsStreamAsync();

                blobStorageClient.CreateFile(path, contents, true);
                resp.StatusCode = HttpStatusCode.Created;
                var location = new Uri(new Uri("asv://" + this.ContainerName + "@" + this.accountNameResolver.FullAccount + "/"), new Uri(path, UriKind.Relative));
                resp.Headers.Add("Location", location.ToString());
            }
            return resp;
        }


        private Task<HttpResponseMessage> GetHomeDirectory(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("BlobStorageAdapter does not support HomeDirectory.");
        }

        private Task<HttpResponseMessage> GetFileChecksum(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("BlobStorageAdapter does not support GetFileChecksum.");
        }

        private Task<HttpResponseMessage> MakeDirs(HttpRequestMessage request, CancellationToken cancellationToken)
        { 
            var resp = new HttpResponseMessage();
            string path = GetAzurePath(request.RequestUri.LocalPath);

            blobStorageClient.CreateDirectory(path);
            resp.StatusCode = HttpStatusCode.Created;
            resp.Content = new StringContent("{\"boolean\": true}", Encoding.Default, "application/json");
            return TaskEx.Run(() => resp);
        }

        private Task<HttpResponseMessage> Rename(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return TaskEx.Run(() => new HttpResponseMessage(HttpStatusCode.NotImplemented));
        }

        private Task<HttpResponseMessage> Delete(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string path = GetAzurePath(request.RequestUri.LocalPath);

            var resp = new HttpResponseMessage();

            BlobType? bt = blobStorageClient.GetTypeIfExists(path);

            if (bt == null)
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent("{\"boolean\": false}", Encoding.Default, "application/json");
            }
            else if (bt == BlobType.File)  // delete file
            {
                blobStorageClient.DeleteFile(path);
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent("{\"boolean\": true}", Encoding.Default, "application/json");
            } 
            else if (bt == BlobType.Directory) // delete directory
            {
                // see if recursive
                var queryStringPairs = request.RequestUri.ParseQueryString();
                bool recursive = false;
                Boolean.TryParse(queryStringPairs["recursive"], out recursive);
                blobStorageClient.DeleteDirectory(path, recursive);
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent("{\"boolean\": true}", Encoding.Default, "application/json");
            }

            return TaskEx.Run(() => resp);
        }

        private Task<HttpResponseMessage> GetContentSummary(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            resp.StatusCode = HttpStatusCode.OK;
            string path = GetAzurePath(request.RequestUri.LocalPath);

            var descriptors = blobStorageClient.ListDirectory(path);

            var content = new JObject(new JProperty("ContentSummary", 
                            new JObject(
                                new JProperty("directoryCount", descriptors.Where(d => d.Type == BlobType.Directory).Count()),
                                new JProperty("fileCount", descriptors.Where(d => d.Type == BlobType.File).Count()),
                                new JProperty("length", descriptors.Where(d => d.Type == BlobType.File).Sum(d => d.Length)),
                                new JProperty("spaceConsumed", descriptors.Where(d => d.Type == BlobType.File).Sum(d => d.Length)))));

            resp.Content = new StringContent(content.ToString(), Encoding.Default, "application/json");

            return TaskEx.Run(() => resp);
        }

        private string GetAzurePath(string path)
        {
            var azurePath = path.Replace("/webhdfs/v1/", "");
            return azurePath.TrimStart('/');
    }
}
}
