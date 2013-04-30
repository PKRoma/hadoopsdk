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

namespace Microsoft.WindowsAzure.Management.HDInsight.RestClient
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.Old;

    internal interface IHDInsightManagementRestClient : IDisposable
    {
        Task<string> ListCloudServices();

        Task CreateContainer(string dnsName, string location, string clusterPayload);

        Task DeleteContainer(string dnsName, string location);
    }
}