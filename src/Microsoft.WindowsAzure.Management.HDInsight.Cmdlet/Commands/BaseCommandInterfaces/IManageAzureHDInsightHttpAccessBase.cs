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
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    internal interface IManageAzureHDInsightHttpAccessBase : IAzureHDInsightClusterCommandBase
    {
        /// <summary>
        ///     Gets or sets Credential to connect to the HDInsight cluster.
        /// </summary>
        PSCredential Credential { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to enable or disable Http services on this Azure HDInsight cluster.
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        ///     Gets or sets the Location for the cluster to return.
        /// </summary>
        string Location { get; set; }
    }
}
