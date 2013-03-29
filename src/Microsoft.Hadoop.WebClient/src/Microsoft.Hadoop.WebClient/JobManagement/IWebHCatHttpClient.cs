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
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for communicating with the WebHCat REST interface.
    /// </summary>
    public interface IWebHCatHttpClient
    {
        /// <summary>
        /// Method to execute a hive job using the WebHCat REST interface.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="statusDir">The directory to store status information.</param>
        /// <returns>The id of the job.</returns>
        Task<string> ExecuteHiveJob(string query, string statusDir);
    }
}
