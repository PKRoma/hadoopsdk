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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces
{
    using System.Management.Automation;

    internal interface INewAzureHDInsightClusterBase : IRemoveAzureHDInsightClusterBase
    {
        /// <summary>
        /// Gets or sets the Asv Account name to use for the cluster's default container.
        /// </summary>
        string DefaultStorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the Asv Account key to use for the cluster's default container.
        /// </summary>
        string DefaultStorageAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the container to use for the cluster's default container.
        /// </summary>
        string DefaultStorageContainerName { get; set; }

        /// <summary>
        /// Gets or sets the user credentials (username and password).
        /// </summary>
        PSCredential Credential { get; set; }

        /// <summary>
        /// Gets or sets the version to use for the cluster.
        /// </summary>
        string Version { get; set; }
        
        /// <summary>
        /// Gets or sets the number of data (worker) nodes to use in the cluster.
        /// </summary>
        int ClusterSizeInNodes { get; set; }
    }
}
