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

namespace Microsoft.Hadoop.WebClient.WebHDFSClient.BlobStorageAdapter
{
    using System;

    internal class StorageAccountNameResolver : IStorageAccountNameResolver
    {
        private const string AzureBlobStorage = @"blob.core.windows.net";
        
        public StorageAccountNameResolver(string accountName)
        {
            this.AccountName = accountName.Split('.')[0];
            if (accountName.Contains("."))
            {
                this.FullAccountUrl = new Uri("http://" + accountName);
            }
            else
            {
                this.FullAccountUrl = new Uri("http://" + accountName + "." + AzureBlobStorage);
            }
        }

        public string AccountName { get; private set; }
        public string FullAccount
        {
            get
            {
                return this.FullAccountUrl.Host;
            }
        }
        public Uri FullAccountUrl { get; private set; }
    }
}
