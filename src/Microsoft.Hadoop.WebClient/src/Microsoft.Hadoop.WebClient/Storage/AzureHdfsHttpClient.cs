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
    using Microsoft.Hadoop.WebHDFS;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AzureHdfsHttpClient : IAzureHdfsHttpClient
    {
        internal string accountName;
        internal string accountKey;
        internal string container;

        public AzureHdfsHttpClient(string accountName, string accountKey, string container)
        {
            this.accountName = accountName;
            this.accountKey = accountKey;
            this.container = container;
        }

        /// <inheritdoc/>
        public Task<HttpContent> OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<string> CreateFile(string path, Stream content, bool overwrite)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> Delete(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DirectoryEntry> GetFileStatus(string path)
        {
            throw new NotImplementedException();
        }
    }
}
