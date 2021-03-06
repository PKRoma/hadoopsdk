﻿// Copyright (c) Microsoft Corporation
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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient
{
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    /// <summary>
    /// A factory interface used to create new HDInsight management POCO clients.
    /// </summary>
    public interface IHDInsightManagementPocoClientFactory
    {
        /// <summary>
        /// Creates a new instance of the IHDInsightManagementPocoClient interface.
        /// This interface can be used to manage an HDInsight cluster.
        /// </summary>
        /// <param name="credentials">
        /// The credentials to use when creating the client.
        /// </param>
        /// <returns>
        /// A new instance of the IHDInsightManagmentPocoClient interface to be used
        /// to manage a cluster.
        /// </returns>
        IHDInsightManagementPocoClient Create(IConnectionCredentials credentials);
    }
}
