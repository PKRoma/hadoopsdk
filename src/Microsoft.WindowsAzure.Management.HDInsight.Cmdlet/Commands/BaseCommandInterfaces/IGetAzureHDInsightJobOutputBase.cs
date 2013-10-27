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
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;

    internal interface IGetAzureHDInsightJobOutputBase : IAzureHDInsightCommonCommandBase
    {
        /// <summary>
        /// Gets or sets the HDInsight cluster to connect to.
        /// </summary>
        string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the Id of the jobDetails to retrieve.
        /// </summary>
        string JobId { get; set; }

        /// <summary>
        /// Gets or sets the type of jobDetails output to retrieve.
        /// </summary>
        JobOutputType OutputType { get; set; }

        /// <summary>
        /// Gets or sets the directory to download task logs to.
        /// </summary>
        string TaskLogsDirectory { get; set; }
    }
}
