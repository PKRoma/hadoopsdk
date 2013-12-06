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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects
{
    /// <summary>
    ///     Provides creation details for a new Pig jobDetails.
    /// </summary>
    public class AzureHDInsightPigJobDefinition : AzureHDInsightJobDefinition
    {
        /// <summary>
        ///     Gets or sets the query file to use for a pig jobDetails.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        ///     Gets or sets the query to use for a pig jobDetails.
        /// </summary>
        public string Query { get; set; }
    }
}
