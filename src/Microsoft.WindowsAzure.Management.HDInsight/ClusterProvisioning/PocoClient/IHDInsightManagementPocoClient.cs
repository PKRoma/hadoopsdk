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

    /// <summary>
    /// Provides an object oriented abstraction over the HDInsight management REST client.
    /// </summary>
    public interface IHDInsightManagementPocoClient : IDisposable
    {
        /// <summary>
        /// Lists the HDInsight containers for a subscription.
        /// </summary>
        /// <returns>
        /// A task that can be used to retrieve a collection of HDInsight containers (clusters).
        /// </returns>
        Task<Collection<HDInsightCluster>> ListContainers();

        /// <summary>
        /// Lists a single HDInsight container by name.
        /// </summary>
        /// <param name="dnsName">
        /// The name of the HDInsight container.
        /// </param>
        /// <returns>
        /// A task that can be used to retrieve the requested HDInsight container.
        /// </returns>
        Task<HDInsightCluster> ListContainer(string dnsName);

        /// <summary>
        /// Creates a new HDInsight container (cluster).
        /// </summary>
        /// <param name="details">
        /// A creation object with the details of how the container should be 
        /// configured.
        /// </param>
        /// <returns>
        /// A task that can be used to wait for the creation request to complete.
        /// </returns>
        Task CreateContainer(HDInsightClusterCreationDetails details);

        /// <summary>
        /// Deletes an HDInsight container (cluster).
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster to delete.
        /// </param>
        /// <returns>
        /// A task that can be used to wait for the delete request to complete.
        /// </returns>
        Task DeleteContainer(string dnsName);

        /// <summary>
        /// Deletes an HDInsight container (cluster).
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster to delete.
        /// </param>
        /// <param name="location">
        /// The location of the cluster to delete.
        /// </param>
        /// <returns>
        /// A task that can be used to wait for the delete request to complete.
        /// </returns>
        Task DeleteContainer(string dnsName, string location);

        /// <summary>
        /// Waits for a cluster to enter a specified condition.
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster.
        /// </param>
        /// <param name="evaluate">
        /// A function that will evaluate the cluster and return true when the cluster is in the given state.
        /// </param>
        /// <param name="interval">
        /// The amount of time to wait between requests.
        /// </param>
        void WaitForClusterCondition(string dnsName, Func<HDInsightCluster, bool> evaluate, TimeSpan interval);
    }
}