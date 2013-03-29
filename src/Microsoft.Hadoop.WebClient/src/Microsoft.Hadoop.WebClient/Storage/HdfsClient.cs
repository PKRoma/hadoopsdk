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
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Class used to work with HDFS.
    /// </summary>
    public class HdfsClient : IHdfsClient
    {
        /// <summary>
        /// HDFS Adapter to use when comunicating with HDFS
        /// </summary>
        internal IHdfsClientAdapter Adapter { get; set; }

        /// <summary>
        /// Initializes a new instance of the HdfsClient class.
        /// </summary>
        /// <param name="adapter">The adapter to use to connect to HDFS.</param>
        private HdfsClient(IHdfsClientAdapter adapter)
        {
            this.Adapter = adapter;
        }

        /// <summary>
        /// Factory method to create a new Hdfs Client conected to an Azure Storage account.
        /// </summary>
        /// <param name="accountName">The storage account name.</param>
        /// <param name="accountKey">The storage account key.</param>
        /// <param name="container">The container to connect to.</param>
        /// <returns>An interface for a HDFSClient.</returns>
        public static IHdfsClient CreateAzureClient(string accountName, string accountKey, string container)
        {
            return new HdfsClient(new AzureHdfsClientAdapter(accountName, accountKey, container));
        }

        /// <summary>
        /// Factory method to create a new Hdfs Client connected to a WebHdfs endpoint.
        /// </summary>
        /// <param name="webHdfsUri">The endpoint to connect to.</param>
        /// <param name="userName">The user name to use.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>An interface for a HDFSClient.</returns>
        public static IHdfsClient CreateWebHdfsClient(Uri webHdfsUri, string userName, string password)
        {
            return new HdfsClient(new WebHdfsClientAdapter(webHdfsUri));
        }

        /// <summary>
        /// Factory method to create a new Hdfs Client connected to a local HDFS instance.
        /// </summary>
        /// <returns>An interface for a HDFSClient.</returns>
        public static IHdfsClient CreateLocalClient()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> CreateFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Stream> OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> Delete(string path, bool recursive)
        {
            throw new NotImplementedException();
        }
    }
}
