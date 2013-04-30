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

namespace Microsoft.WindowsAzure.Management.HDInsight.Data
{
    /// <summary>
    /// Possible states of an HDInsight cluster.
    /// </summary>
    public enum ClusterState
    {
        /// <summary>
        /// Cluster Creation has been registered by the service.
        /// </summary>
        Accepted,

        /// <summary>
        /// Storage Account has been configured for provisioning.
        /// </summary>
        ClusterStorageProvisioned,

        /// <summary>
        /// Configuring the VM nodes.
        /// </summary>
        AzureVMConfiguration,

        /// <summary>
        /// Cluster is in intalling HDInsight on all nodes.
        /// </summary>
        HDInsightConfiguration,

        /// <summary>
        /// Cluster is available for use with 90% of the nodes ready.
        /// </summary>
        Operational,

        /// <summary>
        /// Cluster is available for use with all of the nodes ready.
        /// </summary>
        Running,

        /// <summary>
        /// Cluster is being deleted.
        /// </summary>
        Deleting,

        /// <summary>
        /// Cluster has been deleted.
        /// </summary>
        DeleteQueued,

        /// <summary>
        /// Cluster is in an Unkown state or parsing failed (mismatch between SDK and Service). 
        /// </summary>
        Unknown,

        /// <summary>
        /// Cluster is in a failed state.
        /// </summary>
        Error
    }
}