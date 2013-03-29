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

namespace Microsoft.Hadoop.WebClient.HadoopImplementations
{
    using Microsoft.Hadoop.WebClient.Storage;
    using System;

    public class WebHadoop : IHadoop
    {
        internal WebHadoop(IHdfsClient hdfsFile)
        {
            this.StorageSystem = hdfsFile;
        }

        public static IHadoop Create(Uri clusterName, string userName, string password)
        {
            string clusterAddress = clusterName.Scheme + "://" + clusterName.Host + ":50111";
            string storageAddress = clusterName.Scheme + "://" + clusterName.Host + ":50070";
            IHdfsClient storageSystem = HdfsClient.CreateWebHdfsClient(new Uri(storageAddress), userName, password);
            return new LocalHadoop(storageSystem);
        }

        /// <inheritdoc />
        public IHdfsClient StorageSystem { get; private set; }
    }
}
