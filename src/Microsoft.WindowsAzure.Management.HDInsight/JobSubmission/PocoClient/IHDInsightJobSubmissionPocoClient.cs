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

namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.PocoClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data;

    /// <summary>
    /// Provides POCO (Plan old C# object) representation of the HDInsight job submission rest call.
    /// </summary>
    public interface IHDInsightJobSubmissionPocoClient 
    {
        /// <summary>
        /// Creates a new hive job on the cluster.
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster.
        /// </param>
        /// <param name="location">
        /// The location of the cluster.
        /// </param>
        /// <param name="details">
        /// The details of the job to create on the cluster.
        /// </param>
        /// <returns>
        /// A task representing the cluster creation.
        /// </returns>
        Task<HDInsightJobCreationResults> CreateHiveJob(string dnsName, string location, HDInsightHiveJobCreationDetails details);

        /// <summary>
        /// Creates a new map reduce job on the cluster.
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster.
        /// </param>
        /// <param name="location">
        /// The location of the cluster.
        /// </param>
        /// <param name="details">
        /// The details of the job to create on the cluster.
        /// </param>
        /// <returns>
        /// A task representing the cluster creation.
        /// </returns>
        Task<HDInsightJobCreationResults> CreateMapReduceJob(string dnsName, string location, HDInsightMapReduceJobCreationDetails details);

        /// <summary>
        /// Lists the jobs in the clusters job history.
        /// </summary>
        /// <param name="dnsName">
        /// The name of the cluster.
        /// </param>
        /// <param name="location">
        /// The location of the cluster.
        /// </param>
        /// <returns>
        /// A task that will return the HDInsightJobList containing the jobs.
        /// </returns>
        Task<HDInsightJobList> ListJobs(string dnsName, string location);

        /// <summary>
        /// Gets the details associated with a job.
        /// </summary>
        /// <param name="dnsName">
        /// The dnsName for the cluster.
        /// </param>
        /// <param name="location">
        /// The location of the cluster.
        /// </param>
        /// <param name="jobId">
        /// The jobId to retrieve.
        /// </param>
        /// <returns>
        /// A task that can be used to retrieve the job details.
        /// </returns>
        Task<HDInsightJob> GetJobDetail(string dnsName, string location, string jobId);
    }
}
