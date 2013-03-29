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

namespace Microsoft.Hadoop.WebClient
{
    using Microsoft.Hadoop.WebClient.Storage;

    public interface IHadoop
    {
        /// <summary>
        /// Gets a storage system that can be used to manipulate
        /// files in the distributed system.
        /// </summary>
        IHdfsClient StorageSystem { get; }

        /// <summary>
        /// Gets an <see cref="IStreamingJobExecutor"/> that can be 
        /// used to submit job classes to the server for execution.
        /// </summary>
        //IStreamingJobExecutor MapReduceJob { get; }
    }
}
