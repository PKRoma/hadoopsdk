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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;

    /// <summary>
    /// Client Library that allows interacting with the Azure HDInsight Deployment Service.
    /// </summary>
    public interface IHDInsightClient
    {
        /// <summary>
        /// Gets or sets the polling interval for the CreateContainer\DeleteContainer operations.
        /// </summary>
        TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Queries the HDInsight Clusters registered.
        /// </summary>
        /// <returns>Task that returns a list of HDInsight Clusters.</returns>
        Task<Collection<ListClusterContainerResult>> ListContainers();

        /// <summary>
        /// Queries for a specific HDInsight Cluster registered.
        /// </summary>
        /// <param name="dnsName">DnsName of the HDInsight cluster.</param>
        /// <returns>Task that returns an HDInsight Cluster or NULL if not found.</returns>
        Task<ListClusterContainerResult> ListContainer(string dnsName);

        /// <summary>
        /// Submits a request to create an HDInsight cluster and waits for it to complete.
        /// </summary>
        /// <param name="cluster">Request object that encapsulates all the configurations.</param>
        /// <returns>Object that will manage the deployment and returns an object that represents the HDInsight Cluster created.</returns>
        Task<ListClusterContainerResult> CreateContainer(CreateClusterRequest cluster);

        /// <summary>
        /// Submits a request to delete an HDInsight cluster.
        /// </summary>
        /// <param name="dnsName">DnsName of the HDInsight cluster.</param>
        /// <returns>Task that submits a DeleteCluster request.</returns>
        Task DeleteContainer(string dnsName);
    }
}
