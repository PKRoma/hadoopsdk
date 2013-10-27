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
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    /// <summary>
    /// Provides the base class for an AzureHDInsightJob object.
    /// </summary>
    public class AzureHDInsightJobBase
    {
        /// <summary>
        /// Initializes a new instance of the AzureHDInsightJobBase class.
        /// </summary>
        /// <param name="jobDetails">The HDInsight jobDetails.</param>
        public AzureHDInsightJobBase(JobDetails jobDetails)
        {
            jobDetails.ArgumentNotNull("jobDetails");
            this.JobId = jobDetails.JobId;
        }

        /// <summary>
        /// Gets the JobId returned by the request.
        /// </summary>
        public string JobId { get; private set; }
    }
}
