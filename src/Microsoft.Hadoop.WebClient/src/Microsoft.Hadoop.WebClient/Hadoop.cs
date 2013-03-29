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

namespace Microsoft.Hadoop.WebClient
{
    using System;
    using Microsoft.Hadoop.WebClient.HadoopImplementations;

    internal delegate IHadoop LocalHadoopConstructor();

    internal delegate IHadoop OneBoxHadoopConstructor(Uri clusterName, string userName, string password);

    internal delegate IHadoop AzureHadoopConstructor(Uri clusterName, 
                                                     string userName, 
                                                     string hadoopUser,
                                                     string password, 
                                                     string storageAccount, 
                                                     string storageKey,
                                                     string container,
                                                     bool createContainerIfMissing);

    public static class Hadoop
    {
        internal static LocalHadoopConstructor makeLocal = () => LocalHadoop.Create();
        internal static OneBoxHadoopConstructor makeOneBox = (cluster, user, password) => WebHadoop.Create(cluster, user, password);
        internal static AzureHadoopConstructor makeAzure = (cluster, user, hadoopUser, password, account, key, container, create) =>  
                                                           HadoopOnAzure.Create(cluster, user, hadoopUser, password, account, key, container, create);

        public static IHadoop Connect()
        {
            return makeLocal();
        }

        public static IHadoop Connect(Uri clusterName, string userName, string password)
        {
            return makeOneBox(clusterName, userName, password);
        }

        public static IHadoop Connect(Uri clusterName,
                                      string userName,
                                      string hadoopUser,
                                      string password,
                                      string storageAccount,
                                      string storageKey,
                                      string container,
                                      bool createContainerIfMissing)
        {
            return makeAzure(clusterName, userName, hadoopUser, password, storageAccount, storageKey, container, createContainerIfMissing);
        }
    }
}
