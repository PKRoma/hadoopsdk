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
namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.Storage;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class StorageAbstractionSimulator : IStorageAbstraction
    {
        private readonly WindowsAzureStorageAccountCredentials credentials;
        private WabStorageSimulatorItem root;

        public StorageAbstractionSimulator(WindowsAzureStorageAccountCredentials credentials)
        {
            this.credentials = credentials;
            this.root = new WabStorageSimulatorItem(this.wabsAccount, string.Empty);
        }

        internal Uri wabsAccount
        {
            get
            {
                var rootPath = this.credentials.Name;
                if (!rootPath.Contains("://") && !rootPath.StartsWith(Constants.WabsProtocol))
                {
                    rootPath = string.Format("{0}://{1}", Constants.WabsProtocol, rootPath);
        }

                return new Uri(rootPath);
            }
        }

        private class WabStorageSimulatorItem
        {
            public WabStorageSimulatorItem(Uri path, string Name)
            {
                this.Path = path;
                this.Name = Name;
                this.Items = new Dictionary<string, WabStorageSimulatorItem>();
            }
            public Uri Path { get; private set; }
            public string Name { get; private set; }
            public IDictionary<string, WabStorageSimulatorItem> Items { get; private set; }
            public byte[] Data { get; set; }
        }

        private class PathInfo
        {
            public PathInfo(Uri path)
            {
                this.Protocol = path.Scheme;
                this.Server = path.Host;
                this.Container = path.UserInfo;
                this.IsAbsolute = path.IsAbsoluteUri;
                this.Path = string.Empty;
                this.PathParts = new string[0];
                var localPath = path.LocalPath;
                if (path.LocalPath.StartsWith("/") && path.LocalPath.IsNotNullOrEmpty())
                {
                    localPath = path.LocalPath.Substring(1);
                }
                if (path.LocalPath.EndsWith("/"))
                {
                    localPath = path.LocalPath.Substring(0, path.LocalPath.Length - 1);
                }
                if (localPath.IsNotNullOrEmpty())
                {
                    this.Path = localPath;
                    this.PathParts = this.Path.Split('/');
                }
            }

            public string Protocol { get; private set; }
            public bool IsAbsolute { get; private set; }
            public string Server { get; private set; }
            public string Container { get; private set; }
            public string Path { get; private set; }
            public string[] PathParts { get; private set; }
        }

        private WabStorageSimulatorItem GetItem(PathInfo pathInfo, bool parent = false)
        {
            WabStorageSimulatorItem dir = this.root;
            this.root.Items.TryGetValue(pathInfo.Container, out dir);

            var pathParts = pathInfo.PathParts;
            if (parent)
            {
                if (pathParts.Length > 0)
                {
                    pathParts = pathParts.Take(pathParts.Length - 1).ToArray();
                }
            }

            if (pathParts.Length == 0)
            {
                return dir;
            }

            var loc = 0;
            while (dir.IsNotNull() && dir.Items.TryGetValue(pathParts[loc], out dir) && loc < pathParts.Length)
            {
                if (loc == pathParts.Length - 1)
                {
                    return dir;
                }
                if (dir.IsNull())
                {
                    return null;
                }
                loc++;
            }
            return null;
        }

        private void AssertIsValidWabsUri(PathInfo pathInfo)
        {
            if (!pathInfo.IsAbsolute ||
                pathInfo.Protocol != Constants.WabsProtocol ||
                pathInfo.Container.IsNullOrEmpty() ||
                pathInfo.Server.IsNullOrEmpty() ||
                this.wabsAccount.Host != pathInfo.Server)
            {
                throw new InvalidOperationException("An attempt was made to access content from an invalid storage location or a location not relative to this account.");
            }
        }

        public Task<bool> Exists(Uri path)
        {
            var pathInfo = new PathInfo(path);
            this.AssertIsValidWabsUri(pathInfo);

            var result = this.GetItem(pathInfo);
            return Task.FromResult(result.IsNotNull());
        }

        public void Delete(Uri path)
        {
            var pathInfo = new PathInfo(path);

            if (pathInfo.Path.IsNullOrEmpty())
            {
                throw new InvalidOperationException("An attempt was made to delete a container.  Containers can not be deleted via this API.");
            }
            this.AssertIsValidWabsUri(pathInfo);

            var item = this.GetItem(pathInfo, true);
            if (item.IsNotNull() && item.Items.ContainsKey(pathInfo.PathParts[pathInfo.PathParts.Length - 1]))
            {
                item.Items.Remove(pathInfo.PathParts[pathInfo.PathParts.Length - 1]);
            }
        }

        public Task CreateContainerIfNotExists(string containerName)
        {
            containerName.ArgumentNotNullOrEmpty("containerName");
            var containerUri = new Uri(string.Format("{0}://{1}@{2}", Constants.WabsProtocol, containerName, this.wabsAccount.Host));
            var pathInfo = new PathInfo(containerUri);
            this.AssertIsValidWabsUri(pathInfo);
            this.CreateTree(pathInfo);

            return Task.Delay(0);
        }

        private WabStorageSimulatorItem CreateTree(PathInfo pathInfo)
        {
            var dir = this.root;
            var child = this.root;
            Uri currentUri = new Uri(string.Format("{0}{1}@{2}", Constants.WabsProtocolSchemeName, pathInfo.Container, this.wabsAccount.Host));
            if (!dir.Items.TryGetValue(pathInfo.Container, out dir))
            {
                var containerUri = currentUri.Scheme + "://" + pathInfo.Container + "@" + currentUri.Host;
                dir = new WabStorageSimulatorItem(new Uri(containerUri), pathInfo.Container);
                this.root.Items.Add(pathInfo.Container, dir);
            }
            foreach (var pathPart in pathInfo.PathParts)
            {
                var uri = this.FixUriEnding(currentUri);
                currentUri = new Uri(uri + "/" + pathPart);
                if (!dir.Items.TryGetValue(pathPart, out child))
                {
                    child = new WabStorageSimulatorItem(currentUri, pathPart);
                    dir.Items.Add(pathPart, child);
                    dir = child;
                }
                else
                {
                    dir = child;
                }
            }
            return dir;
        }

        private string FixUriEnding(Uri uri)
        {
            var asString = uri.ToString();
            if (asString.EndsWith("/"))
            {
                asString = asString.Substring(0, asString.Length - 1);
            }
            return asString;
        }

        public Task Write(Uri path, Stream stream)
        {
            var pathInfo = new PathInfo(path);
            if (pathInfo.Path.IsNullOrEmpty())
            {
                throw new InvalidOperationException("An attempt was made write but no path was provided.  Data can not be written without a path.");
            }
            this.AssertIsValidWabsUri(pathInfo);
            var item = this.CreateTree(pathInfo);
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                item.Data = new byte[mem.Length];
                mem.Position = 0;
                mem.Read(item.Data, 0, item.Data.Length);
            }
            return Task.Delay(0);
        }

        public Task<Stream> Read(Uri path)
        {
            var pathInfo = new PathInfo(path);
            this.AssertIsValidWabsUri(pathInfo);
            var item = this.GetItem(pathInfo);
            if (item.IsNull())
            {
                throw new InvalidOperationException("Attempt to read an item that was not present in the container.");
            }
            Stream stream = Help.SafeCreate<MemoryStream>();
            stream.Write(item.Data, 0, item.Data.Length);
            stream.Position = 0;
            return Task.FromResult(stream);
        }

        public Task<IEnumerable<Uri>> List(Uri path, bool recursive)
        {
            List<Uri> items = new List<Uri>();
            Queue<WabStorageSimulatorItem> queue = new Queue<WabStorageSimulatorItem>();
            var pathInfo = new PathInfo(path);
            this.AssertIsValidWabsUri(pathInfo);
            var item = this.GetItem(pathInfo, pathInfo.Path.IsNullOrEmpty());
            if (item.IsNotNull())
            {
                queue.Enqueue(item);
                while (queue.Count > 0)
                {
                    item = queue.Remove();
                    queue.AddRange(item.Items.Values);
                    items.Add(item.Path);
                }
            }
            return Task.FromResult((IEnumerable<Uri>)items);
        }
    }
}
