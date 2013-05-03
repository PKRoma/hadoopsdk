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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;

    internal interface IHDInsightManagementPocoClient : IDisposable
    {
        Task<Collection<ListClusterContainerResult>> ListContainers();

        Task<ListClusterContainerResult> ListContainer(string dnsName);

        Task CreateContainer(CreateClusterRequest cluster);

        Task DeleteContainer(string dnsName);

        Task DeleteContainer(string dnsName, string location);

        void WaitForClusterCondition(string dnsName, Func<ListClusterContainerResult, bool> evaluate, TimeSpan interval);
    }
}