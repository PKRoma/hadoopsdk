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
    ///     Enumeration of possible jobDetails output types.
    /// </summary>
    public enum JobOutputType
    {
        /// <summary>
        ///     Specifies that the jobDetails output file to download is the stdout file.
        /// </summary>
        StandardOutput,

        /// <summary>
        ///     Specifies that the jobDetails output file to download is the stderr file.
        /// </summary>
        StandardError,

        /// <summary>
        ///     Specifies that the jobDetails output file to download is the logs/list.txt file.
        /// </summary>
        TaskSummary,

        /// <summary>
        ///     Specifies that the jobDetails files under logs/ folder should be downloaded.
        /// </summary>
        TaskLogs
    }
}
