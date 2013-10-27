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
    using System.Collections.Generic;

    internal interface INewAzureHDInsightHiveJobDefinitionBase : INewAzureHDInsightJobWithDefinesConfigBase
    {
        /// <summary>
        /// Gets or sets the query file to use for a hive job.
        /// </summary>
        string File { get; set; }

        /// <summary>
        /// Gets or sets the Query to use for the jobDetails.
        /// </summary>
        string Query { get; set; }

        /// <summary>
        /// Gets or sets the Arguments for the jobDetails.
        /// </summary>
        string[] Arguments { get; set; }
    }
}
