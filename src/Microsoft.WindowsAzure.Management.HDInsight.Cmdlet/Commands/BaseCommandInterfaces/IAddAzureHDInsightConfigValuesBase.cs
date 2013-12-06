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
    using System.Collections;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;

    /// <summary>
    ///     Represents a command to set custom configuration values for Hadoop services.
    /// </summary>
    internal interface IAddAzureHDInsightConfigValuesBase
    {
        /// <summary>
        ///     Gets or sets the AzureHDInsightConfig.
        /// </summary>
        AzureHDInsightConfig Config { get; set; }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Core Hadoop service.
        /// </summary>
        Hashtable Core { get; set; }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Hdfs Hadoop service.
        /// </summary>
        Hashtable Hdfs { get; set; }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Hive Hadoop service.
        /// </summary>
        AzureHDInsightHiveConfiguration Hive { get; set; }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the MapReduce Hadoop service.
        /// </summary>
        AzureHDInsightMapReduceConfiguration MapReduce { get; set; }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Oozie Hadoop service.
        /// </summary>
        AzureHDInsightOozieConfiguration Oozie { get; set; }
    }
}
