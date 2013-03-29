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
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a job inside the current Hadoop context.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Starts execution of the job.
        /// </summary>
        /// <returns>A task for execution.</returns>
        Task Execute();

        /// <summary>
        /// Determines if the job has completed.
        /// </summary>
        /// <returns>A value that is true if the job has completed.</returns>
        Task<bool> HasJobCompleted();

        /// <summary>
        /// Gets the results of the job.
        /// </summary>
        /// <returns>A stream that contains the results of the job.</returns>
        Task<Stream> GetJobResults();

    }
}
