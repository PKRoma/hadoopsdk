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
    using System.Collections.Generic;

    /// <summary>
    ///     Provides creation details for a new Hive jobDetails.
    /// </summary>
    public class AzureHDInsightHiveJobDefinition : AzureHDInsightJobDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the AzureHDInsightHiveJobDefinition class.
        /// </summary>
        public AzureHDInsightHiveJobDefinition()
        {
            this.Defines = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Gets the parameters for the jobDetails.
        /// </summary>
        public IDictionary<string, string> Defines { get; private set; }

        /// <summary>
        ///     Gets or sets the query file to use for a hive jobDetails.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        ///     Gets or sets the name of the jobDetails.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        ///     Gets or sets the query to use for a hive jobDetails.
        /// </summary>
        public string Query { get; set; }
    }
}
