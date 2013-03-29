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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Client adapter that communicates with WebHDFS.
    /// </summary>
    public class WebHdfsClientAdapter : IHdfsClientAdapter
    {
        /// <summary>
        /// Client for communicating with the WebHdfs REST interface.
        /// </summary>
        internal IWebHdfsHttpClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the the WebHdfsClientAdapter class.
        /// </summary>
        /// <param name="webHdfsUri">The Uri for the target WebHdfs REST interface.</param>
        public WebHdfsClientAdapter(Uri webHdfsUri)
        {
            this.Client = new WebHdfsHttpClient(webHdfsUri);
        }

        /// <inheritdoc/>
        public Task<bool> CreateDirectory(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<string> DeleteDirectory(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<object>> GetDirectory(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> CreateFile(string path, Stream content)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Stream> OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> AppendToFile(string path, Stream content)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> FileExists(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<object>> GetFiles(string path)
        {
            throw new NotImplementedException();
        }
    }
}
