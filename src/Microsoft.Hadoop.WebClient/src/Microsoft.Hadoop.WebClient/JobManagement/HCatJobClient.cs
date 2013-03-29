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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Communicates with HCat to submit and manage jobs.
    /// </summary>
    public class HCatJobClient : IHCatJobClient
    {
        /// <summary>
        /// Client to use when communicating with HCat
        /// </summary>
        internal IWebHCatHttpClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the HCatJobClient class.
        /// </summary>
        /// <param name="webHcatEndpoint">The endpoint to connect to.</param>
        private HCatJobClient(Uri webHcatEndpoint)
        {
            this.Client = new WebHCatHttpClient(webHcatEndpoint);
        }

        /// <summary>
        /// Factory method to create a new HCat Client.
        /// </summary>
        /// <param name="webHcatEndpoint">The endpoint to connect to.</param>
        /// <returns>A HCatClient object.</returns>
        public static IHCatJobClient Create(Uri webHcatEndpoint)
        {
            return new HCatJobClient(webHcatEndpoint);
        }

        /// <inheritdoc/>
        public Task SubmitHiveJob()
        {
            throw new System.NotImplementedException();
        }
    }
}
