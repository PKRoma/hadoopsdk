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

namespace Microsoft.Hadoop.WebClient.JobManagement
{
    /// <summary>
    /// Manages jobs for the current Hadoop instance.
    /// </summary>
    public interface IJobManager
    {
        /// <summary>
        /// Factory method to construct a hive job.
        /// </summary>
        /// <param name="query">The query for the job to execute.</param>
        /// <returns>A Job object.</returns>
        IJob CreateHiveJob(string query);
    }
}
